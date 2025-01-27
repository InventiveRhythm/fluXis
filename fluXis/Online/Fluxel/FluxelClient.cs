#nullable enable

using System;
using System.Threading;
using System.Threading.Tasks;
using fluXis.Configuration;
using fluXis.Graphics.Sprites;
using fluXis.Online.Activity;
using fluXis.Online.API;
using fluXis.Online.API.Models.Chat;
using fluXis.Online.API.Models.Other;
using fluXis.Online.API.Models.Users;
using fluXis.Online.API.Requests.Auth;
using fluXis.Online.Notifications;
using fluXis.Overlay.Notifications;
using Midori.Networking.WebSockets;
using Midori.Networking.WebSockets.Frame;
using Midori.Networking.WebSockets.Typed;
using Newtonsoft.Json.Linq;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Development;
using osu.Framework.Graphics;
using osu.Framework.Logging;

namespace fluXis.Online.Fluxel;

public partial class FluxelClient : Component, IAPIClient, INotificationClient
{
    [Resolved]
    private NotificationManager notifications { get; set; } = null!;

    private Bindable<bool> logResponses = null!;
    public bool LogResponses => logResponses.Value;

    public APIEndpointConfig Endpoint { get; }

    public Bindable<APIUser?> User { get; } = new();
    public Bindable<ConnectionStatus> Status { get; } = new();
    public Exception? LastException { get; private set; }

    public Bindable<UserActivity> Activity { get; } = new();

    public long MaintenanceTime { get; set; }

    #region Events

    public event Action<APIUser>? FriendOnline;
    public event Action<APIUser>? FriendOffline;
    public event Action<Achievement>? AchievementEarned;
    public event Action<ServerMessage>? MessageReceived;
    public event Action<string>? ChatChannelAdded;
    public event Action<string>? ChatChannelRemoved;
    public event Action<APIChatMessage>? ChatMessageReceived;
    public event Action<string, string>? ChatMessageRemoved;

    #endregion

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
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        Activity.BindValueChanged(e =>
        {
            var activity = e.NewValue;

            if (activity is null || connection?.State != WebSocketState.Open)
                return;

            connection.Server.UpdateActivity(activity.GetType().Name, JObject.FromObject(activity));
        });
    }

    internal new void Schedule(Action action) => base.Schedule(action);

    #region Socket Connect

    private TypedWebSocketClient<INotificationServer, INotificationClient>? connection;

    private void loop()
    {
        while (true)
        {
            if (Status.Value == ConnectionStatus.Closed)
                break;

            if (Status.Value is ConnectionStatus.Failed or ConnectionStatus.Authenticating)
            {
                Thread.Sleep(100);
                continue;
            }

            if (!hasValidCredentials)
            {
                Status.Value = ConnectionStatus.Offline;
                Thread.Sleep(100);
                continue;
            }

            if (connection is null)
                tryConnect();

            switch (Status.Value)
            {
                // if we're not online, we can't do anything
                case ConnectionStatus.Failed
                    or ConnectionStatus.Closed
                    or ConnectionStatus.Offline:
                    continue;
            }

            Thread.Sleep(50);
        }
    }

    private void tryConnect()
    {
        if (Status.Value == ConnectionStatus.Reconnecting)
            Thread.Sleep(5000);

        Logger.Log("Connecting to server...", LoggingTarget.Network);

        try
        {
            connection = GetWebSocket<INotificationServer, INotificationClient>(this, "/notifications");
            connection.OnClose += () =>
            {
                Status.Value = Status.Value == ConnectionStatus.Online ? ConnectionStatus.Reconnecting : ConnectionStatus.Failed;
                connection = null;
            };

            Logger.Log("Connected to server! Waiting for authentication...", LoggingTarget.Network);

            // this is needed when using a local server
            // else it will fail to connect most of the time
            if (DebugUtils.IsDebugBuild)
                Thread.Sleep(500);

            var waitTime = 5d;

            // ReSharper disable once AsyncVoidLambda
            var task = new Task(async () =>
            {
                while (Status.Value == ConnectionStatus.Connecting && waitTime > 0)
                {
                    if (connection.State is WebSocketState.Closing or WebSocketState.Closed)
                    {
                        LastException = new APIException("" /*connection.CloseStatusDescription*/);
                        Status.Value = ConnectionStatus.Failed;
                        break;
                    }

                    waitTime -= 0.1;
                    await Task.Delay(100);
                }

                if (Status.Value == ConnectionStatus.Online) return;

                Logger.Log("Authentication timed out!", LoggingTarget.Network);
                Logout();

                LastException = new TimeoutException("Authentication timed out!");
                Status.Value = ConnectionStatus.Failed;
            });

            task.Start();
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Failed to connect to server!", LoggingTarget.Network);
            LastException = ex;
            connection = null;

            if (Status.Value == ConnectionStatus.Reconnecting)
                return;

            Status.Value = ConnectionStatus.Failed;
        }
    }

    public void Disconnect()
    {
        if (connection is { State: WebSocketState.Open })
            connection.Close(WebSocketCloseCode.NormalClosure, "Client closed");

        connection = null;
        Status.Value = ConnectionStatus.Closed;
    }

    public TypedWebSocketClient<S, C> GetWebSocket<S, C>(C target, string path)
        where S : class where C : class
    {
        var socket = new TypedWebSocketClient<S, C>(target) { PingInterval = 30000 };
        socket.RequestHeaders["Authorization"] = AccessToken;
        socket.Connect((Endpoint.APIUrl + path).Replace("http", "ws")); // this MIGHT be janky
        return socket;
    }

    #endregion

    #region Authentication

    public bool IsLoggedIn => User.Value != null;

    public string AccessToken => tokenBindable.Value;
    public string MultifactorToken { get; set; } = "";
    private Bindable<string> tokenBindable = null!;

    private Bindable<string> username = null!;

    private bool hasValidCredentials => !string.IsNullOrEmpty(AccessToken);

    public async void Login(string username, string password)
    {
        Logger.Log("Logging in...", LoggingTarget.Network);
        this.username.Value = username;

        Status.Value = ConnectionStatus.Authenticating;

        var req = new LoginRequest(username, password);
        await PerformRequestAsync(req);

        if (!req.IsSuccessful)
        {
            Logger.Log($"Failed to get access token! ({req.FailReason?.Message})", LoggingTarget.Network, LogLevel.Error);
            LastException = req.FailReason;
            Status.Value = ConnectionStatus.Failed;
            return;
        }

        tokenBindable.Value = req.Response.Data!.AccessToken;
        Status.Value = ConnectionStatus.Connecting;
    }

    public async void Register(string username, string password, string email)
    {
        Logger.Log("Registering account...", LoggingTarget.Network);
        this.username.Value = username;

        Status.Value = ConnectionStatus.Authenticating;

        var req = new RegisterRequest(username, password, email);
        await PerformRequestAsync(req);

        if (!req.IsSuccessful)
        {
            Logger.Log($"Failed to register account! ({req.FailReason?.Message})", LoggingTarget.Network, LogLevel.Error);
            LastException = req.FailReason;
            Status.Value = ConnectionStatus.Failed;
            return;
        }

        tokenBindable.Value = req.Response.Data!.AccessToken;
        Status.Value = ConnectionStatus.Connecting;
    }

    public void Logout()
    {
        connection?.Close(WebSocketCloseCode.NormalClosure, "Logout");
        connection = null;

        User.Value = null;
        tokenBindable.Value = "";

        Status.Value = ConnectionStatus.Offline;
    }

    #endregion

    #region Requests

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

    #endregion

    #region INotificationClient Implementation

    Task INotificationClient.Login(APIUser user)
    {
        User.Value = user;
        Status.Value = ConnectionStatus.Online;
        return Task.CompletedTask;
    }

    Task INotificationClient.Logout(string reason)
    {
        Logout();
        notifications.SendText("You have been logged out!", reason, FontAwesome6.Solid.TriangleExclamation);
        return Task.CompletedTask;
    }

    Task INotificationClient.NotifyFriendStatus(APIUser friend, bool online)
    {
        if (online)
            FriendOnline?.Invoke(friend);
        else
            FriendOffline?.Invoke(friend);

        return Task.CompletedTask;
    }

    Task INotificationClient.RewardAchievement(Achievement achievement)
    {
        AchievementEarned?.Invoke(achievement);
        return Task.CompletedTask;
    }

    Task INotificationClient.DisplayMessage(ServerMessage message)
    {
        MessageReceived?.Invoke(message);
        return Task.CompletedTask;
    }

    Task INotificationClient.DisplayMaintenance(long time)
    {
        if (time > DateTimeOffset.UtcNow.ToUnixTimeSeconds())
            MaintenanceTime = time;

        return Task.CompletedTask;
    }

    Task INotificationClient.ReceiveChatMessage(APIChatMessage message)
    {
        ChatMessageReceived?.Invoke(message);
        return Task.CompletedTask;
    }

    Task INotificationClient.DeleteChatMessage(string channel, string id)
    {
        ChatMessageRemoved?.Invoke(channel, id);
        return Task.CompletedTask;
    }

    Task INotificationClient.AddToChatChannel(string channel)
    {
        ChatChannelAdded?.Invoke(channel);
        return Task.CompletedTask;
    }

    Task INotificationClient.RemoveFromChatChannel(string channel)
    {
        ChatChannelRemoved?.Invoke(channel);
        return Task.CompletedTask;
    }

    #endregion
}

public enum ConnectionStatus
{
    Offline,
    Failed,
    Authenticating,
    Connecting,
    Online,
    Reconnecting,
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
    MultiplayerScore,
    MultiplayerFinish
}
