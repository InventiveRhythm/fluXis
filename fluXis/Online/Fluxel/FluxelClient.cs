#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using fluXis.Configuration;
using fluXis.Online.Activity;
using fluXis.Online.API;
using fluXis.Online.API.Models.Chat;
using fluXis.Online.API.Models.Notifications;
using fluXis.Online.API.Models.Other;
using fluXis.Online.API.Models.Users;
using fluXis.Online.API.Requests;
using fluXis.Online.API.Requests.Auth;
using fluXis.Online.API.Requests.Users;
using fluXis.Online.Collections;
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
    public bool CanUseOnline { get; protected set; }

    public EndpointConfig Endpoint { get; private set; } = new(string.Empty);
    private string configUrl = string.Empty;

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
    public event Action<string, List<CollectionItem>, List<CollectionItem>, List<string>>? OnCollectionUpdated;

    #endregion

    [BackgroundDependencyLoader]
    private void load(FluXisConfig config)
    {
        configUrl = config.Get<string>(FluXisSetting.ServerUrl);
        username = config.GetBindable<string>(FluXisSetting.Username);
        tokenBindable = config.GetBindable<string>(FluXisSetting.Token);
        logResponses = config.GetBindable<bool>(FluXisSetting.LogAPIResponses);
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

    public void UpdateLastRead()
    {
        if (connection?.State != WebSocketState.Open)
            return;

        var time = DateTimeOffset.Now.ToUnixTimeSeconds();
        connection.Server.UpdateNotificationUnread(time);
        LastReadTime = time;
    }

    #endregion

    #region Socket Connect

    private TypedWebSocketClient<INotificationServer, INotificationClient>? connection;

    public void PullServerConfig(Action complete, Action<Exception> failure)
    {
        try
        {
            Endpoint = new EndpointConfig(configUrl);

            var req = new ServerConfigRequest();
            req.Failure += fail;
            req.Success += res =>
            {
                Logger.Log($"Pulled config for {configUrl}.", LoggingTarget.Network);

                Endpoint = new EndpointConfig(configUrl, res.Data);
                CanUseOnline = true;

                complete();
            };

            PerformRequestAsync(req);
        }
        catch (Exception ex)
        {
            fail(ex);
        }

        void fail(Exception ex)
        {
            Logger.Error(ex, "Failed to pull server config!", LoggingTarget.Network);
            failure.Invoke(ex);
        }
    }

    public void TryConnecting()
    {
        if (!HasCredentials)
            return;

        var thread = new Thread(loop) { IsBackground = true };
        thread.Start();
    }

    private void loop()
    {
        while (true)
        {
            if (Status.Value == ConnectionStatus.Closed)
                break;

            if (Status.Value is ConnectionStatus.Failed or ConnectionStatus.Authenticating or ConnectionStatus.Offline)
            {
                Thread.Sleep(100);
                continue;
            }

            if (!HasCredentials)
                throw new InvalidOperationException("In connect loop without credentials?");

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
                Status.Value = ConnectionStatus.Reconnecting;
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
                        Status.Value = ConnectionStatus.Reconnecting;
                        break;
                    }

                    waitTime -= 1;
                    await Task.Delay(TimeSpan.FromSeconds(1));
                }

                if (Status.Value == ConnectionStatus.Online) return;

                Logger.Log("Authentication timed out!", LoggingTarget.Network);
                Logger.Log($"Connection status is {Status.Value}.", LoggingTarget.Network, LogLevel.Debug);

                LastException = new TimeoutException("Authentication timed out!");
                Status.Value = ConnectionStatus.Reconnecting;
            });

            task.Start();
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Failed to connect to server!", LoggingTarget.Network);
            LastException = ex;
            connection = null;
            Status.Value = ConnectionStatus.Reconnecting;
        }
    }

    public void Disconnect()
    {
        if (connection is { State: WebSocketState.Open })
            connection.Close(WebSocketCloseCode.NormalClosure, "Client closed");

        connection = null;
        Status.Value = ConnectionStatus.Closed;
    }

    public void DisableOnline()
    {
        CanUseOnline = false;
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
    public bool HasCredentials => !string.IsNullOrEmpty(AccessToken);

    public string AccessToken => tokenBindable.Value;
    public string MultifactorToken { get; set; } = "";
    private Bindable<string> tokenBindable = null!;

    private Bindable<string> username = null!;

    public async Task<Exception?> Login(string username, string password)
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
            return req.FailReason;
        }

        tokenBindable.Value = req.Response.Data!.AccessToken;
        return await ReLogin();
    }

    public async Task<Exception?> ReLogin()
    {
        Logger.Log("Logging in with saved token...", LoggingTarget.Network);

        Status.Value = ConnectionStatus.Authenticating;

        var req = new UserSelfRequest();
        await PerformRequestAsync(req);

        if (!req.IsSuccessful)
        {
            Logger.Error(req.FailReason, "Failed to log in with existing token!", LoggingTarget.Network);
            LastException = req.FailReason;
            Status.Value = ConnectionStatus.Failed;
            tokenBindable.Value = "";
            return req.FailReason;
        }

        User.Value = req.Response.Data;
        username.Value = User.Value.Username;
        Status.Value = ConnectionStatus.Connecting;
        return null;
    }

    public async Task<Exception?> Register(string username, string password, string email)
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
            return req.FailReason;
        }

        tokenBindable.Value = req.Response.Data!.AccessToken;
        return await ReLogin();
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
        // Logout();
        // notifications.SendText("You have been logged out!", reason, FontAwesome6.Solid.TriangleExclamation);
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

    Task INotificationClient.CollectionUpdated(string id, List<CollectionItem> added, List<CollectionItem> changed, List<string> removed)
    {
        OnCollectionUpdated?.Invoke(id, added, changed, removed);
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
