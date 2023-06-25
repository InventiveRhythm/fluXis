using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using fluXis.Game.Configuration;
using fluXis.Game.Online.API;
using fluXis.Game.Online.Chat;
using fluXis.Game.Online.Fluxel.Packets;
using fluXis.Game.Online.Fluxel.Packets.Account;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using osu.Framework.Graphics;
using osu.Framework.IO.Network;
using osu.Framework.Logging;

namespace fluXis.Game.Online.Fluxel;

public partial class Fluxel : Component
{
    public FluXisConfig Config { get; }
    public APIEndpointConfig Endpoint { get; }

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

    public Action<APIUserShort> OnUserLoggedIn { get; set; }

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

    public bool HasValidCredentials => !string.IsNullOrEmpty(Token) || (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password));

    public APIUserShort LoggedInUser
    {
        get => loggedInUser;
        set
        {
            if (value == null)
                Logger.Log("Logged out", LoggingTarget.Network);
            else
            {
                loggedInUser = value;
                OnUserLoggedIn?.Invoke(loggedInUser);
                Logger.Log($"Logged in as {value.Username}", LoggingTarget.Network);
            }
        }
    }

    public string LastError { get; private set; }
    public string Token { get; private set; }

    public Fluxel(FluXisConfig config, APIEndpointConfig endpoint)
    {
        this.Config = config;
        Endpoint = endpoint;

        Token = config.Get<string>(FluXisSetting.Token);

        var thread = new Thread(loop) { IsBackground = true };
        thread.Start();

        RegisterListener<string>(EventType.Token, onAuthResponse);
        RegisterListener<APIUserShort>(EventType.Login, onLoginResponse);
        RegisterListener<APIRegisterResponse>(EventType.Register, onRegisterResponse);
    }

    private async void loop()
    {
        while (true)
        {
            if (Status == ConnectionStatus.Failing)
                Thread.Sleep(5000);

            if (!HasValidCredentials)
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

                await SendPacket(new RegisterPacket
                {
                    Username = username,
                    Password = password,
                    Email = email
                });
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

        await Send(packet);
    }

    private async Task receive()
    {
        Logger.Log("Waiting for data...", LoggingTarget.Network);

        if (connection.State == WebSocketState.Open)
        {
            try
            {
                // receive data
                byte[] buffer = new byte[2048];
                await connection.ReceiveAsync(buffer, CancellationToken.None);

                // convert to string
                string message = System.Text.Encoding.UTF8.GetString(buffer).TrimEnd('\0');
                Logger.Log(message, LoggingTarget.Network);

                // handler logic
                void handleListener<T>()
                {
                    var response = FluxelResponse<T>.Parse(message);

                    if (responseListeners.ContainsKey(response.Type))
                    {
                        foreach (var listener
                                 in (IEnumerable<Action<object>>)responseListeners.GetValueOrDefault(response.Type)
                                    ?? ArraySegment<Action<object>>.Empty)
                        {
                            listener(response);
                        }
                    }
                }

                // find right handler
                Action handler = (EventType)JsonConvert.DeserializeObject<JObject>(message)["id"]!.ToObject<int>() switch
                {
                    EventType.Token => handleListener<string>,
                    EventType.Login => handleListener<APIUserShort>,
                    EventType.Register => handleListener<APIRegisterResponse>,
                    EventType.ChatMessage => handleListener<ChatMessage>,
                    _ => () => { }
                };
                // execute handler
                handler();
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
        Token = null;
        username = null;
        password = null;

        Config.GetBindable<string>(FluXisSetting.Token).Value = "";
    }

    public async Task Send(string message)
    {
        if (connection is not { State: WebSocketState.Open })
        {
            packetQueue.Add(message);
            return;
        }

        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(message);
        await connection.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
    }

    public async void SendPacketAsync(Packet packet) => await SendPacket(packet);

    public async Task SendPacket(Packet packet)
    {
        FluxelRequest request = new FluxelRequest(packet.ID, packet);
        string json = JsonConvert.SerializeObject(request);
        await Send(json);
    }

    public void RegisterListener<T>(EventType id, Action<FluxelResponse<T>> listener)
    {
        responseListeners.GetOrAdd(id, _ => new List<Action<object>>()).Add(response => listener((FluxelResponse<T>)response));
    }

    public void UnregisterListener(EventType id)
    {
        responseListeners.Remove(id, out var listeners);
        listeners?.Clear();
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
    }

    public WebRequest CreateAPIRequest(string url, HttpMethod method)
    {
        var request = new WebRequest($"{Endpoint.APIUrl}{url}");
        request.AllowInsecureRequests = true;
        request.Method = method;
        request.AddHeader("Authorization", Token);
        return request;
    }

    private void onAuthResponse(FluxelResponse<string> response)
    {
        if (response.Status == 200)
        {
            Token = response.Data;
            Config.GetBindable<string>(FluXisSetting.Token).Value = Token;
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

        Token = response.Data.Token;
        Config.GetBindable<string>(FluXisSetting.Token).Value = Token;
        LoggedInUser = response.Data.User;
        registering = false;
        Status = ConnectionStatus.Online;
    }
}

public enum ConnectionStatus
{
    Offline,
    Connecting,
    Online,
    Reconnecting,
    Failing
}

public enum EventType
{
    Token = 0,
    Login = 1,
    Register = 2,

    ChatMessage = 10,

    MultiplayerCreateLobby = 20,
    MultiplayerJoinLobby = 21,
    MultiplayerLobbyUpdate = 22,
}
