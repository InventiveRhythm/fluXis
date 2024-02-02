using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using fluXis.Game.Configuration;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Online.API.Models.Account;
using fluXis.Game.Online.API.Models.Multi;
using fluXis.Game.Online.API.Models.Other;
using fluXis.Game.Online.API.Models.Users;
using fluXis.Game.Online.Chat;
using fluXis.Game.Online.Fluxel.Packets;
using fluXis.Game.Online.Fluxel.Packets.Account;
using fluXis.Game.Online.Fluxel.Packets.Multiplayer;
using fluXis.Game.Overlay.Notifications;
using fluXis.Game.Utils;
using Newtonsoft.Json.Linq;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.IO.Network;
using osu.Framework.Logging;

namespace fluXis.Game.Online.Fluxel;

public partial class Fluxel : Component
{
    public APIEndpointConfig Endpoint { get; }

    [Resolved]
    private NotificationManager notifications { get; set; }

    private string username;
    private string password;
    private string email;
    private double waitTime;
    private bool registering;

    private readonly List<string> packetQueue = new();
    private readonly ConcurrentDictionary<EventType, List<Action<object>>> responseListeners = new();
    private ClientWebSocket connection;
    private ConnectionStatus status = ConnectionStatus.Offline;
    private APIUserShort loggedInUser;

    public Action<APIUserShort> OnUserChanged { get; set; }

    public ConnectionStatus Status
    {
        get => status;
        private set
        {
            if (status == value) return;

            status = value;
            Logger.Log($"Status changed to {value}", LoggingTarget.Network);
            OnStatusChanged?.Invoke(value);
        }
    }

    public Action<ConnectionStatus> OnStatusChanged { get; set; }

    private bool hasValidCredentials => !string.IsNullOrEmpty(Token) || (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password));

    public APIUserShort LoggedInUser
    {
        get => loggedInUser;
        private set
        {
            loggedInUser = value;
            OnUserChanged?.Invoke(loggedInUser);

            Logger.Log(value == null ? "Logged out" : $"Logged in as {value.Username}", LoggingTarget.Network);
        }
    }

    public string LastError { get; private set; }

    public string Token => tokenBindable.Value;
    private Bindable<string> tokenBindable;

    public Fluxel(APIEndpointConfig endpoint)
    {
        Endpoint = endpoint;
    }

    [BackgroundDependencyLoader]
    private void load(FluXisConfig config)
    {
        tokenBindable = config.GetBindable<string>(FluXisSetting.Token);

        var thread = new Thread(loop) { IsBackground = true };
        thread.Start();

        RegisterListener<string>(EventType.Token, onAuthResponse);
        RegisterListener<APIUserShort>(EventType.Login, onLoginResponse);
        RegisterListener<APIRegisterResponse>(EventType.Register, onRegisterResponse);
        RegisterListener<object>(EventType.Logout, onLogout);
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
        // ReSharper disable once FunctionNeverReturns
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
                    await SendPacket(new AuthPacket(username, password));
                else
                    await SendPacket(new LoginPacket(Token));
            }
            else
            {
                Logger.Log("Registering...", LoggingTarget.Network);
                waitTime = 10;

                if (string.IsNullOrEmpty(email))
                    throw new Exception("Email is required for registration!");

                await SendPacket(new RegisterPacket(username, password, email));
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
        Logger.Log("Waiting for data...", LoggingTarget.Network);

        if (connection.State == WebSocketState.Open)
        {
            try
            {
                string message = "";
                bool end = false;

                while (!end) // incomplete packet, wait for more data
                {
                    // receive data
                    byte[] buffer = new byte[1024 * 1024 * 2]; // 2MB (some packets are big)
                    var res = await connection.ReceiveAsync(buffer, CancellationToken.None);

                    // check if end of message
                    end = res.EndOfMessage;

                    // convert to string
                    string msg = System.Text.Encoding.UTF8.GetString(buffer).TrimEnd('\0');
                    message += msg;
                }

                if (string.IsNullOrEmpty(message)) return;

                Logger.Log(message, LoggingTarget.Network);

                // handler logic
                void handleListener<T>(string msg)
                {
                    var response = msg.Deserialize<FluxelResponse<T>>();

                    var type = getType(response.Type);

                    if (!responseListeners.ContainsKey(type)) return;

                    foreach (var listener
                             in (IEnumerable<Action<object>>)responseListeners.GetValueOrDefault(type)
                                ?? ArraySegment<Action<object>>.Empty)
                    {
                        listener(response);
                    }
                }

                var idString = message.Deserialize<JObject>()["id"]!.ToObject<string>();
                Logger.Log($"Received packet {idString}", LoggingTarget.Network, LogLevel.Debug);

                // find right handler
                Action<string> handler = getType(idString) switch
                {
                    EventType.Token => handleListener<string>,
                    EventType.Login => handleListener<APIUserShort>,
                    EventType.Register => handleListener<APIRegisterResponse>,
                    EventType.Logout => handleListener<object>,
                    EventType.Achievement => handleListener<Achievement>,
                    EventType.ServerMessage => handleListener<ServerMessage>,
                    EventType.ChatMessage => handleListener<ChatMessage>,
                    EventType.ChatHistory => handleListener<ChatMessage[]>,
                    EventType.ChatMessageDelete => handleListener<string>,
                    EventType.MultiplayerJoin => handleListener<MultiplayerJoinPacket>,
                    EventType.MultiplayerLeave => handleListener<MultiplayerLeavePacket>,
                    EventType.MultiplayerRoomUpdate => handleListener<MultiplayerRoomUpdate>,
                    EventType.MultiplayerReady => handleListener<MultiplayerReadyUpdate>,
                    EventType.MultiplayerStartGame => handleListener<dynamic>,
                    _ => _ => { }
                };

                // execute handler
                handler(message);
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

    public async void Login(string username, string password)
    {
        this.username = username;
        this.password = password;

        await SendPacket(new AuthPacket(username, password));
    }

    public void Register(string username, string password, string email)
    {
        this.username = username;
        this.password = password;
        this.email = email;
        registering = true;
    }

    public async void Logout()
    {
        await connection.CloseAsync(WebSocketCloseStatus.NormalClosure, "Logout Requested", CancellationToken.None);
        LoggedInUser = null;
        tokenBindable.Value = "";
        username = "";
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

        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(message);
        await connection.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
    }

    public async void SendPacketAsync(Packet packet) => await SendPacket(packet);

    public async Task SendPacket(Packet packet)
    {
        FluxelRequest request = new FluxelRequest(packet.ID, packet);
        await send(request.Serialize());
    }

    public void RegisterListener<T>(EventType id, Action<FluxelResponse<T>> listener)
    {
        responseListeners.GetOrAdd(id, _ => new List<Action<object>>()).Add(response => listener((FluxelResponse<T>)response));
    }

    public void UnregisterListener<T>(EventType id, Action<FluxelResponse<T>> listener)
    {
        if (responseListeners.TryGetValue(id, out var listeners))
            listeners.Remove(response => listener((FluxelResponse<T>)response));
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

    private void onAuthResponse(FluxelResponse<string> response)
    {
        if (response.Status == 200)
        {
            tokenBindable.Value = response.Data;
            waitTime = 5; // reset wait time for login
            SendPacketAsync(new LoginPacket(Token));
        }
        else
        {
            Logout();
            LastError = response.Message;
            Status = ConnectionStatus.Failing;
        }
    }

    private void onLoginResponse(FluxelResponse<APIUserShort> response)
    {
        if (response.Status == 200)
        {
            LoggedInUser = response.Data;
            Status = ConnectionStatus.Online;
        }
        else
        {
            Logout();
            LastError = response.Message;
            Status = ConnectionStatus.Failing;
        }
    }

    private void onRegisterResponse(FluxelResponse<APIRegisterResponse> response)
    {
        if (response.Status != 200)
        {
            Logout();
            LastError = response.Message;
            Status = ConnectionStatus.Failing;
            return;
        }

        tokenBindable.Value = response.Data.Token;
        LoggedInUser = response.Data.User;
        registering = false;
        Status = ConnectionStatus.Online;
    }

    private void onLogout(FluxelResponse<object> response)
    {
        Logout();
        notifications.SendText("You have been logged out!", "Another device logged in with your account.", FontAwesome6.Solid.TriangleExclamation);
    }

    private EventType getType(string id)
    {
        return id switch
        {
            "account/auth" => EventType.Token,
            "account/login" => EventType.Login,
            "account/register" => EventType.Register,
            "account/logout" => EventType.Logout,

            "achievement" => EventType.Achievement,
            "server/message" => EventType.ServerMessage,

            "chat/message" => EventType.ChatMessage,
            "chat/history" => EventType.ChatHistory,
            "chat/delete" => EventType.ChatMessageDelete,

            "multi/create" => EventType.MultiplayerCreateLobby,
            "multi/join" => EventType.MultiplayerJoin,
            "multi/leave" => EventType.MultiplayerLeave,
            "multi/update" => EventType.MultiplayerRoomUpdate,
            "multi/ready" => EventType.MultiplayerReady,
            "multi/start" => EventType.MultiplayerStartGame,

            _ => throw new ArgumentOutOfRangeException(nameof(id), id, null)
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

    Achievement,
    ServerMessage,

    ChatMessage,
    ChatHistory,
    ChatMessageDelete,

    MultiplayerCreateLobby,
    MultiplayerJoin,
    MultiplayerLeave,
    MultiplayerRoomUpdate,
    MultiplayerReady,
    MultiplayerStartGame
}
