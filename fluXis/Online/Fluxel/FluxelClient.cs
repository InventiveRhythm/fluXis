#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using fluXis.Configuration;
using fluXis.Graphics.Sprites;
using fluXis.Online.Activity;
using fluXis.Online.API;
using fluXis.Online.API.Models.Chat;
using fluXis.Online.API.Models.Notifications;
using fluXis.Online.API.Models.Other;
using fluXis.Online.API.Models.Users;
using fluXis.Online.API.Requests;
using fluXis.Online.API.Requests.Auth;
using fluXis.Online.Notifications;
using fluXis.Overlay.Notifications;
using fluXis.Utils.Extensions;
using Midori.Networking.WebSockets;
using Midori.Networking.WebSockets.Frame;
using Midori.Networking.WebSockets.Typed;
using Newtonsoft.Json.Linq;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
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
    public event Action? NameChangeRequested;
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

        Status.BindValueChanged(v => Scheduler.ScheduleIfNeeded(() =>
        {
            HasUnreadNotifications.Value = false;

            if (v.NewValue == ConnectionStatus.Online)
                RefreshNotifications().WaitAsync(CancellationToken.None);
        }));
    }

    internal new void Schedule(Action action) => base.Schedule(action);

    #region Notifations

    public Bindable<bool> HasUnreadNotifications { get; } = new();
    public List<APINotification> CurrentNotifications { get; } = new();
    public event Action<APINotification>? NewNotification;
    public long LastReadTime { get; set; }

    public async Task RefreshNotifications()
    {
        CurrentNotifications.Clear();

        var req = new NotificationsRequest();
        await PerformRequestAsync(req);

        if (!req.IsSuccessful)
            return;

        var notif = req.Response.Data.Notifications;
        LastReadTime = req.Response.Data.LastRead;

        if (notif.Any(x => x.Time > LastReadTime))
            HasUnreadNotifications.Value = true;

        CurrentNotifications.AddRange(notif);
    }

    #endregion

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
        else
            Status.Value = ConnectionStatus.Connecting;

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

            var waitTime = 10d;

            // ReSharper disable once AsyncVoidLambda
            var task = new Task(async () =>
            {
                while (Status.Value is ConnectionStatus.Connecting or ConnectionStatus.Reconnecting && waitTime > 0)
                {
                    if (connection.State >= WebSocketState.Closing)
                    {
                        LastException = new APIException(connection.CloseReason);
                        Status.Value = ConnectionStatus.Failed;
                        break;
                    }

                    waitTime -= 1;
                    await Task.Delay(TimeSpan.FromSeconds(1));
                }

                if (Status.Value == ConnectionStatus.Online) return;

                Logger.Log("Authentication timed out!", LoggingTarget.Network);
                Logger.Log($"Connection status is {Status.Value}.", LoggingTarget.Network, LogLevel.Debug);
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
        socket.RequestHeaders["X-Version"] = FluXisGameBase.VersionString;

        try
        {
            socket.Connect((Endpoint.APIUrl + path).Replace("http", "ws")); // this MIGHT be janky
            return socket;
        }
        catch (Exception ex)
        {
            throw new AggregateException(socket.CloseReason, ex);
        }
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
            Logger.Error(req.FailReason, "Failed to get access token!", LoggingTarget.Network);
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
            Logger.Error(req.FailReason, "Failed to register account!", LoggingTarget.Network);
            LastException = req.FailReason;
            Status.Value = ConnectionStatus.Failed;
            return;
        }

        tokenBindable.Value = req.Response.Data!.AccessToken;
        Status.Value = ConnectionStatus.Connecting;
    }

    public void Logout()
    {
        Logger.Log("Logging out...");
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

    public Task NotificationReceived(APINotification notification)
    {
        Scheduler.ScheduleIfNeeded(() =>
        {
            HasUnreadNotifications.Value = true;
            CurrentNotifications.Add(notification);
            NewNotification?.Invoke(notification);
        });

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

    Task INotificationClient.ForceNameChange()
    {
        NameChangeRequested?.Invoke();
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
