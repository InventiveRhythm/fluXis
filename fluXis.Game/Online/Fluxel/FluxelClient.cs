using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using fluXis.Game.Configuration;
using fluXis.Game.Graphics.Sprites;
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
using osu.Framework.Graphics;
using osu.Framework.IO.Network;
using osu.Framework.Logging;

namespace fluXis.Game.Online.Fluxel;

public partial class FluxelClient : Component
{
    private const int chunk_out = 4096; // 4KB
    private const int chunk_in = 1024; // 1KB (its actually 1016 bytes server-side, but we'll round up)

    public APIEndpointConfig Endpoint { get; }

    [Resolved]
    private NotificationManager notifications { get; set; }

    public string Token => tokenBindable.Value;
    private Bindable<string> tokenBindable;

    private Bindable<string> username;
    private string password;
    private string email;
    private double waitTime;
    private bool registering;

    private bool hasValidCredentials => !string.IsNullOrEmpty(Token) || (!string.IsNullOrEmpty(username.Value) && !string.IsNullOrEmpty(password));

    private readonly List<string> packetQueue = new();
    private readonly ConcurrentDictionary<EventType, List<Action<object>>> responseListeners = new();
    private readonly Dictionary<Guid, Action<object>> waitingForResponse = new();
    private ClientWebSocket connection;
    private ConnectionStatus status = ConnectionStatus.Offline;
    private APIUserShort loggedInUser;

    public Action<APIUserShort> OnUserChanged { get; set; }
    public Action<ConnectionStatus> OnStatusChanged { get; set; }

    public ConnectionStatus Status
    {
        get => status;
        private set
        {
            if (status == value)
                return;

            status = value;
            Logger.Log($"Network status changed to {value}!", LoggingTarget.Network);
            OnStatusChanged?.Invoke(value);
        }
    }

    public APIUserShort LoggedInUser
    {
        get => loggedInUser;
        private set
        {
            loggedInUser = value;

            Logger.Log(value == null ? "Logged out!" : $"Logged in as {value.Username}!", LoggingTarget.Network);
            OnUserChanged?.Invoke(loggedInUser);
        }
    }

    public string LastError { get; private set; }

    public FluxelClient(APIEndpointConfig endpoint)
    {
        Endpoint = endpoint;
    }

    [BackgroundDependencyLoader]
    private void load(FluXisConfig config)
    {
        username = config.GetBindable<string>(FluXisSetting.Username);
        tokenBindable = config.GetBindable<string>(FluXisSetting.Token);

        var thread = new Thread(loop) { IsBackground = true };
        thread.Start();

        RegisterListener<AuthPacket>(EventType.Token, onAuthResponse);
        RegisterListener<LoginPacket>(EventType.Login, onLoginResponse);
        RegisterListener<RegisterPacket>(EventType.Register, onRegisterResponse);
        RegisterListener<LogoutPacket>(EventType.Logout, onLogout);
    }

    private async void loop()
    {
        while (true)
        {
            if (Status == ConnectionStatus.Closed)
                break;

            if (Status == ConnectionStatus.Failing)
                Thread.Sleep(5000);

            if (!hasValidCredentials)
            {
                Status = ConnectionStatus.Offline;
                Thread.Sleep(100);
                continue;
            }

            if (Status != ConnectionStatus.Online && Status != ConnectionStatus.Connecting)
                await tryConnect();

            await receive();

            if (waitTime <= 0)
                await processQueue();

            Thread.Sleep(50);
        }
    }

    private async Task tryConnect()
    {
        Status = ConnectionStatus.Connecting;

        Logger.Log("Connecting to server...", LoggingTarget.Network);

        try
        {
            connection = new ClientWebSocket();
            await connection.ConnectAsync(new Uri(Endpoint.WebsocketUrl), CancellationToken.None);
            Logger.Log("Connected to server!", LoggingTarget.Network);

            if (!registering)
            {
                Logger.Log("Logging in...", LoggingTarget.Network);
                waitTime = 5;

                if (string.IsNullOrEmpty(Token))
                    await SendPacket(AuthPacket.CreateC2S(username.Value, password));
                else
                    await SendPacket(LoginPacket.CreateC2S(Token));
            }
            else
            {
                Logger.Log("Registering...", LoggingTarget.Network);
                waitTime = 10;

                if (string.IsNullOrEmpty(email))
                    throw new Exception("Email is required for registration!");

                await SendPacket(RegisterPacket.CreateC2S(username.Value, email, password));
            }

            // ReSharper disable once AsyncVoidLambda
            var task = new Task(async () =>
            {
                while (Status == ConnectionStatus.Connecting && waitTime > 0)
                {
                    waitTime -= 0.1;
                    await Task.Delay(100);
                }

                if (Status != ConnectionStatus.Connecting) return;

                Logger.Log("Login timed out!", LoggingTarget.Network);
                Logout();

                LastError = "Login timed out!";
                Status = ConnectionStatus.Failing;
            });

            task.Start();
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Failed to connect to server!", LoggingTarget.Network);

            LastError = ex.Message;
            Status = ConnectionStatus.Failing;
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
                LastError = e.Message;
            }
        }
        else
        {
            Status = ConnectionStatus.Reconnecting;
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
            Logger.Log($"Received packet {msg}!", LoggingTarget.Network, LogLevel.Debug);
            var response = msg.Deserialize<FluxelReply<T>>();
            var type = getType(response.ID);

            if (waitingForResponse.TryGetValue(response.Token, out var action))
            {
                action(response);
                waitingForResponse.Remove(response.Token);
                return;
            }

            if (!responseListeners.ContainsKey(type))
                return;

            foreach (var listener
                     in (IEnumerable<Action<object>>)responseListeners.GetValueOrDefault(type)
                        ?? ArraySegment<Action<object>>.Empty)
            {
                listener(response);
            }
        }

        var idString = message.Deserialize<FluxelReply<EmptyPacket>>().ID;
        Logger.Log($"Received packet {idString}!", LoggingTarget.Network);

        // find right handler
        Action<string> handler = getType(idString) switch
        {
            EventType.Token => handleListener<AuthPacket>,
            EventType.Login => handleListener<LoginPacket>,
            EventType.Register => handleListener<RegisterPacket>,
            EventType.Logout => handleListener<LogoutPacket>,

            EventType.Achievement => handleListener<AchievementPacket>,
            EventType.ServerMessage => handleListener<ServerMessagePacket>,

            EventType.FriendOnline => handleListener<FriendOnlinePacket>,
            EventType.FriendOffline => handleListener<FriendOnlinePacket>,

            EventType.ChatMessage => handleListener<ChatMessagePacket>,
            EventType.ChatHistory => handleListener<ChatHistoryPacket>,
            EventType.ChatMessageDelete => handleListener<ChatDeletePacket>,

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

    public async void Login(string username, string password)
    {
        this.username.Value = username;
        this.password = password;

        await SendPacket(AuthPacket.CreateC2S(username, password));
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
        LoggedInUser = null;
        tokenBindable.Value = "";
        password = "";

        Status = ConnectionStatus.Offline;
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

    public async void SendPacketAsync(IPacket packet) => await SendPacket(packet);

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

    public void Reset()
    {
        loggedInUser = null;
        responseListeners.Clear();
        packetQueue.Clear();
    }

    public void Close()
    {
        if (connection is { State: WebSocketState.Open })
            connection?.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client closed", CancellationToken.None);

        Status = ConnectionStatus.Closed;
    }

    public WebRequest CreateAPIRequest(string url, HttpMethod method = null)
    {
        method ??= HttpMethod.Get;

        var request = new WebRequest($"{Endpoint.APIUrl}{url}")
        {
            AllowInsecureRequests = true,
            Method = method
        };

        if (!string.IsNullOrEmpty(Token))
            request.AddHeader("Authorization", Token);

        return request;
    }

    internal new void Schedule(Action action) => base.Schedule(action);

    private void onAuthResponse(FluxelReply<AuthPacket> reply)
    {
        if (!reply.Success)
        {
            Logout();
            LastError = reply.Message;
            Status = ConnectionStatus.Failing;
            return;
        }

        tokenBindable.Value = reply.Data!.Token;
        waitTime = 5; // reset wait time for login
        SendPacketAsync(LoginPacket.CreateC2S(reply.Data.Token));
    }

    private void onLoginResponse(FluxelReply<LoginPacket> reply)
    {
        if (!reply.Success)
        {
            Logout();
            LastError = reply.Message;
            Status = ConnectionStatus.Failing;
            return;
        }

        LoggedInUser = reply.Data.User;
        Status = ConnectionStatus.Online;
    }

    private void onRegisterResponse(FluxelReply<RegisterPacket> reply)
    {
        if (!reply.Success)
        {
            Logout();
            LastError = reply.Message;
            Status = ConnectionStatus.Failing;
            return;
        }

        tokenBindable.Value = reply.Data!.Token;
        LoggedInUser = reply.Data.User;
        registering = false;
        Status = ConnectionStatus.Online;
    }

    private void onLogout(FluxelReply<LogoutPacket> reply)
    {
        Logout();
        notifications.SendText("You have been logged out!", "Another device logged in with your account.", FontAwesome6.Solid.TriangleExclamation);
    }

    private static EventType getType(string id)
    {
        return id switch
        {
            PacketIDs.AUTH => EventType.Token,
            PacketIDs.LOGIN => EventType.Login,
            PacketIDs.REGISTER => EventType.Register,
            PacketIDs.LOGOUT => EventType.Logout,

            PacketIDs.ACHIEVEMENT => EventType.Achievement,
            PacketIDs.SERVER_MESSAGE => EventType.ServerMessage,

            PacketIDs.FRIEND_ONLINE => EventType.FriendOnline,
            PacketIDs.FRIEND_OFFLINE => EventType.FriendOffline,

            PacketIDs.CHAT_MESSAGE => EventType.ChatMessage,
            PacketIDs.CHAT_HISTORY => EventType.ChatHistory,
            PacketIDs.CHAT_DELETE => EventType.ChatMessageDelete,

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
    Token,
    Login,
    Register,

    /// <summary>
    /// Logged out by the server, because the same account logged in somewhere else.
    /// </summary>
    Logout,

    FriendOnline,
    FriendOffline,

    Achievement,
    ServerMessage,

    ChatMessage,
    ChatHistory,
    ChatMessageDelete,

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
