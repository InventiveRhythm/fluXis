using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using fluXis.Game.Configuration;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Online.API;
using fluXis.Game.Online.API.Requests.Auth;
using fluXis.Game.Overlay.Notifications;
using fluXis.Shared.API;
using fluXis.Shared.API.Packets;
using fluXis.Shared.API.Packets.Account;
using fluXis.Shared.API.Packets.Chat;
using fluXis.Shared.API.Packets.Multiplayer;
using fluXis.Shared.API.Packets.Other;
using fluXis.Shared.API.Packets.User;
using fluXis.Shared.Components.Users;
using fluXis.Shared.Utils;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Development;
using osu.Framework.Graphics;
using osu.Framework.Logging;

#nullable enable

namespace fluXis.Game.Online.Fluxel;

public partial class FluxelClient : Component, IAPIClient
{
    private const int chunk_out = 4096; // 4KB
    private const int chunk_in = 1024; // 1KB (its actually 1016 bytes server-side, but we'll round up)

    public APIEndpointConfig Endpoint { get; }

    [Resolved]
    private NotificationManager notifications { get; set; } = null!;

    public string AccessToken => tokenBindable.Value;
    public string MultifactorToken { get; set; } = "";
    private Bindable<string> tokenBindable = null!;

    private Bindable<bool> logResponses = null!;
    public bool LogResponses => logResponses.Value;

    private Bindable<string> username = null!;
    private string password = null!;
    private string email = null!;
    private double waitTime;
    private bool registering;

    private bool hasValidCredentials => !string.IsNullOrEmpty(AccessToken) || (!string.IsNullOrEmpty(username.Value) && !string.IsNullOrEmpty(password));

    private readonly List<string> packetQueue = new();
    private readonly ConcurrentDictionary<EventType, List<Action<object>>> responseListeners = new();
    private readonly Dictionary<Guid, Action<object>> waitingForResponse = new();
    private ClientWebSocket connection = null!;

    public Bindable<APIUser?> User { get; } = new();
    public Bindable<ConnectionStatus> Status { get; } = new();
    public Exception? LastException { get; private set; } = null!;

    public long MaintenanceTime { get; set; }

    public bool IsLoggedIn => User.Value != null;

    public FluxelClient(APIEndpointConfig endpoint)
    {
        Endpoint = endpoint;
    }

    [BackgroundDependencyLoader]
    private void load(FluXisConfig config)
    {
        username = config.GetBindable<string>(FluXisSetting.Username);
        tokenBindable = config.GetBindable<string>(FluXisSetting.Token);
        logResponses = config.GetBindable<bool>(FluXisSetting.LogAPIResponses);

        var thread = new Thread(loop) { IsBackground = true };
        thread.Start();

        RegisterListener<LoginPacket>(EventType.Login, onLoginResponse);
        RegisterListener<LogoutPacket>(EventType.Logout, onLogout);
        RegisterListener<MaintenancePacket>(EventType.Maintenance, onMaintenance);
    }

    private async void loop()
    {
        while (true)
        {
            if (Status.Value == ConnectionStatus.Closed)
                break;

            if (Status.Value == ConnectionStatus.Failing)
                Thread.Sleep(5000);

            if (!hasValidCredentials)
            {
                Status.Value = ConnectionStatus.Offline;
                Thread.Sleep(100);
                continue;
            }

            if (Status.Value != ConnectionStatus.Online && Status.Value != ConnectionStatus.Connecting)
                await tryConnect();

            // if we're not online, we can't do anything
            if (Status.Value is ConnectionStatus.Failing
                or ConnectionStatus.Closed
                or ConnectionStatus.Offline)
                continue;

            await receive();

            if (waitTime <= 0)
                await processQueue();

            Thread.Sleep(50);
        }
    }

    private async Task tryConnect()
    {
        Status.Value = ConnectionStatus.Connecting;

        if (registering)
        {
            Logger.Log("Registering account...", LoggingTarget.Network);

            var req = new RegisterRequest(username.Value, password, email);
            await PerformRequestAsync(req);

            if (!req.IsSuccessful)
            {
                Logger.Log($"Failed to register account! ({req.FailReason?.Message})", LoggingTarget.Network, LogLevel.Error);
                LastException = req.FailReason;
                Status.Value = ConnectionStatus.Failing;
                registering = false;
                password = "";
                return;
            }

            registering = false;
            tokenBindable.Value = req.Response.Data!.AccessToken;
        }

        if (string.IsNullOrEmpty(AccessToken))
        {
            Logger.Log("Getting access token...", LoggingTarget.Network);

            var req = new LoginRequest(username.Value, password);
            await PerformRequestAsync(req);

            if (!req.IsSuccessful)
            {
                Logger.Log($"Failed to get access token! ({req.FailReason?.Message})", LoggingTarget.Network, LogLevel.Error);
                LastException = req.FailReason;
                Status.Value = ConnectionStatus.Failing;
                password = ""; // clear password to prevent further attempts
                return;
            }

            tokenBindable.Value = req.Response.Data!.AccessToken;
        }

        Logger.Log("Connecting to server...", LoggingTarget.Network);

        try
        {
            connection = new ClientWebSocket();
            await connection.ConnectAsync(new Uri(Endpoint.WebsocketUrl), CancellationToken.None);
            Logger.Log("Connected to server! Authenticating...", LoggingTarget.Network);

            // this is needed when using a local server
            // else it will fail to connect most of the time
            if (DebugUtils.IsDebugBuild)
                Thread.Sleep(500);

            waitTime = 5;
            await SendPacket(LoginPacket.CreateC2S(AccessToken));

            // ReSharper disable once AsyncVoidLambda
            var task = new Task(async () =>
            {
                while (Status.Value == ConnectionStatus.Connecting && waitTime > 0)
                {
                    waitTime -= 0.1;
                    await Task.Delay(100);
                }

                if (Status.Value != ConnectionStatus.Connecting) return;

                Logger.Log("Authentication timed out!", LoggingTarget.Network);
                Logout();

                LastException = new TimeoutException("Authentication timed out!");
                Status.Value = ConnectionStatus.Failing;
            });

            task.Start();
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Failed to connect to server!", LoggingTarget.Network);

            LastException = ex;
            Status.Value = ConnectionStatus.Failing;
        }
    }

    private async Task processQueue()
    {
        if (packetQueue.Count == 0) return;

        var packet = packetQueue[0];
        packetQueue.RemoveAt(0);

        await send(packet);
    }

    private async Task receive()
    {
        Logger.Log("Waiting for data...", LoggingTarget.Network, LogLevel.Debug);

        if (connection.State == WebSocketState.Open)
        {
            try
            {
                var buffer = new byte[chunk_in];
                var message = new StringBuilder();

                while (true)
                {
                    var result = await connection.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    message.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));

                    if (!result.EndOfMessage)
                        continue;

                    handleMessage(message.ToString());
                    break;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "Something went wrong!", LoggingTarget.Network);
                LastException = e;
            }
        }
        else
        {
            Status.Value = ConnectionStatus.Reconnecting;
            Logger.Log("Reconnecting to server...", LoggingTarget.Network);
        }
    }

    private void handleMessage(string message)
    {
        if (string.IsNullOrEmpty(message))
            return;

                // handler logic
        void handleListener<T>(string msg)
            where T : IPacket
        {
            if (LogResponses)
                Logger.Log($"Received packet {msg}!", LoggingTarget.Network);

            var response = msg.Deserialize<FluxelReply<T>>();

            if (response == null)
                return;

            var type = getType(response.ID);

            if (waitingForResponse.TryGetValue(response.Token, out var action))
            {
                action(response);
                waitingForResponse.Remove(response.Token);
                return;
            }

            if (!responseListeners.TryGetValue(type, out var callbacks))
                return;

            callbacks.ForEach(c => c(response));
        }

        var packet = message.Deserialize<FluxelReply<EmptyPacket>>();

        if (packet == null)
            return;

        var idString = packet.ID;
        Logger.Log($"Received packet {idString}!", LoggingTarget.Network);

        // find right handler
        Action<string> handler = getType(idString) switch
        {
            EventType.Login => handleListener<LoginPacket>,
            EventType.Logout => handleListener<LogoutPacket>,

            EventType.Achievement => handleListener<AchievementPacket>,
            EventType.ServerMessage => handleListener<ServerMessagePacket>,
            EventType.Maintenance => handleListener<MaintenancePacket>,

            EventType.FriendOnline => handleListener<FriendOnlinePacket>,
            EventType.FriendOffline => handleListener<FriendOnlinePacket>,

            EventType.ChatMessage => handleListener<ChatMessagePacket>,
            EventType.ChatHistory => handleListener<ChatHistoryPacket>,
            EventType.ChatMessageDelete => handleListener<ChatDeletePacket>,
            EventType.ChatJoin => handleListener<ChatChannelJoinPacket>,
            EventType.ChatLeave => handleListener<ChatChannelLeavePacket>,

            EventType.MultiplayerCreateLobby => handleListener<MultiCreatePacket>,
            EventType.MultiplayerJoin => handleListener<MultiJoinPacket>,
            EventType.MultiplayerLeave => handleListener<MultiLeavePacket>,
            EventType.MultiplayerState => handleListener<MultiStatePacket>,
            EventType.MultiplayerMap => handleListener<MultiMapPacket>,
            // EventType.MultiplayerRoomUpdate => handleListener<MultiplayerRoomUpdate>,
            EventType.MultiplayerReady => handleListener<MultiReadyPacket>,
            EventType.MultiplayerStartGame => handleListener<MultiStartPacket>,
            EventType.MultiplayerFinish => handleListener<MultiFinishPacket>,
            _ => _ => { }
        };

        // execute handler
        handler(message);
    }

    public void Login(string username, string password)
    {
        Logger.Log("Logging in...", LoggingTarget.Network);
        this.username.Value = username;
        this.password = password;
    }

    public void Register(string username, string password, string email)
    {
        this.username.Value = username;
        this.password = password;
        this.email = email;
        registering = true;
    }

    public async void Logout()
    {
        await connection.CloseAsync(WebSocketCloseStatus.NormalClosure, "Logout Requested", CancellationToken.None);
        User.Value = null;
        tokenBindable.Value = "";
        password = "";

        Status.Value = ConnectionStatus.Offline;
    }

    private async Task send(string message)
    {
        if (connection is not { State: WebSocketState.Open })
        {
            packetQueue.Add(message);
            return;
        }

        Logger.Log($"Sending packet {message}", LoggingTarget.Network, LogLevel.Debug);

        var bytes = Encoding.UTF8.GetBytes(message);

        try
        {
            var length = bytes.Length;
            var sections = length / chunk_out;

            if (length % chunk_out != 0)
                sections++;

            for (var i = 0; i < sections; i++)
            {
                var start = i * chunk_out;
                var end = Math.Min(start + chunk_out, length);

                await connection.SendAsync(new ArraySegment<byte>(bytes, start, end - start), WebSocketMessageType.Text, i == sections - 1, CancellationToken.None);
            }
        }
        catch (Exception e)
        {
            Logger.Error(e, "Failed to send packet!", LoggingTarget.Network);
        }
    }

    /// <summary>
    /// Sends a packet and waits for a response. Throws a <see cref="TimeoutException"/> if the time set in <paramref name="timeout"/> is exceeded.
    /// </summary>
    /// <param name="packet">The packet to send.</param>
    /// <param name="timeout">The time to wait for a response in milliseconds.</param>
    /// <typeparam name="T">The type of the packet.</typeparam>
    /// <returns>The response packet.</returns>
    public async Task<FluxelReply<T>> SendAndWait<T>(T packet, long timeout = 10000)
        where T : IPacket
    {
        var request = new FluxelRequest<T>(packet.ID, packet);

        var task = new TaskCompletionSource<FluxelReply<T>>();
        waitingForResponse.Add(request.Token, response => task.SetResult((FluxelReply<T>)response));

        await send(request.Serialize());
        await Task.WhenAny(task.Task, Task.Delay((int)timeout));

        if (!task.Task.IsCompleted)
            throw new TimeoutException("The request timed out!");

        return await task.Task;
    }

    public async void SendPacketAsync<T>(T packet)
        where T : IPacket => await SendPacket(packet);

    public async Task SendPacket<T>(T packet)
        where T : IPacket
    {
        var request = new FluxelRequest<T>(packet.ID, packet);
        await send(request.Serialize());
    }

    public void RegisterListener<T>(EventType id, Action<FluxelReply<T>> listener)
        where T : IPacket
    {
        responseListeners.GetOrAdd(id, _ => new List<Action<object>>()).Add(response => listener((FluxelReply<T>)response));
    }

    public void UnregisterListener<T>(EventType id, Action<FluxelReply<T>> listener)
        where T : IPacket
    {
        if (responseListeners.TryGetValue(id, out var listeners))
            listeners.Remove(response => listener((FluxelReply<T>)response));
    }

    public void Disconnect()
    {
        if (connection is { State: WebSocketState.Open })
            connection.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client closed", CancellationToken.None);

        Status.Value = ConnectionStatus.Closed;
    }

    public void PerformRequest(APIRequest request)
    {
        try
        {
            request.Perform(this);
        }
        catch (Exception e)
        {
            request.Fail(e);
        }
    }

    public Task PerformRequestAsync(APIRequest request)
        => Task.Factory.StartNew(() => PerformRequest(request));

    internal new void Schedule(Action action) => base.Schedule(action);

    private void onLoginResponse(FluxelReply<LoginPacket> reply)
    {
        if (!reply.Success)
        {
            Logout();
            LastException = new APIException(reply.Message);
            Status.Value = ConnectionStatus.Failing;
            return;
        }

        User.Value = reply.Data!.User;
        Status.Value = ConnectionStatus.Online;
    }

    private void onLogout(FluxelReply<LogoutPacket> reply)
    {
        Logout();
        notifications.SendText("You have been logged out!", "Another device logged in with your account.", FontAwesome6.Solid.TriangleExclamation);
    }

    private void onMaintenance(FluxelReply<MaintenancePacket> e)
    {
        if (e.Data!.Time <= DateTimeOffset.UtcNow.ToUnixTimeSeconds())
            return;

        MaintenanceTime = e.Data.Time;
    }

    private static EventType getType(string id)
    {
        return id switch
        {
            PacketIDs.LOGIN => EventType.Login,
            PacketIDs.LOGOUT => EventType.Logout,

            PacketIDs.ACHIEVEMENT => EventType.Achievement,
            PacketIDs.SERVER_MESSAGE => EventType.ServerMessage,
            PacketIDs.MAINTENANCE => EventType.Maintenance,

            PacketIDs.FRIEND_ONLINE => EventType.FriendOnline,
            PacketIDs.FRIEND_OFFLINE => EventType.FriendOffline,

            PacketIDs.CHAT_MESSAGE => EventType.ChatMessage,
            PacketIDs.CHAT_HISTORY => EventType.ChatHistory,
            PacketIDs.CHAT_DELETE => EventType.ChatMessageDelete,
            PacketIDs.CHAT_JOIN => EventType.ChatJoin,
            PacketIDs.CHAT_LEAVE => EventType.ChatLeave,

            PacketIDs.MULTIPLAYER_CREATE => EventType.MultiplayerCreateLobby,
            PacketIDs.MULTIPLAYER_JOIN => EventType.MultiplayerJoin,
            PacketIDs.MULTIPLAYER_LEAVE => EventType.MultiplayerLeave,
            PacketIDs.MULTIPLAYER_STATE => EventType.MultiplayerState,
            PacketIDs.MULTIPLAYER_MAP => EventType.MultiplayerMap,
            PacketIDs.MULTIPLAYER_UPDATE => EventType.MultiplayerRoomUpdate,
            PacketIDs.MULTIPLAYER_READY => EventType.MultiplayerReady,
            PacketIDs.MULTIPLAYER_START => EventType.MultiplayerStartGame,
            PacketIDs.MULTIPLAYER_FINISH => EventType.MultiplayerFinish,

            _ => throw new ArgumentOutOfRangeException(nameof(id), id, "Unknown packet ID!")
        };
    }
}

public enum ConnectionStatus
{
    Offline,
    Connecting,
    Online,
    Reconnecting,
    Failing,
    Closed
}

public enum EventType
{
    Login,
    Logout, // Logged out by the server, because the same account logged in somewhere else.

    FriendOnline,
    FriendOffline,

    Achievement,
    ServerMessage,
    Maintenance,

    ChatMessage,
    ChatHistory,
    ChatMessageDelete,
    ChatJoin,
    ChatLeave,

    MultiplayerCreateLobby,
    MultiplayerJoin,
    MultiplayerLeave,
    MultiplayerState,
    MultiplayerMap,
    MultiplayerRoomUpdate,
    MultiplayerReady,
    MultiplayerStartGame,
    MultiplayerFinish
}
