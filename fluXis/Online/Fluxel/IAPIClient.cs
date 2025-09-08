using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using fluXis.Online.Activity;
using fluXis.Online.API;
using fluXis.Online.API.Models.Chat;
using fluXis.Online.API.Models.Notifications;
using fluXis.Online.API.Models.Other;
using fluXis.Online.API.Models.Users;
using fluXis.Online.Collections;
using Midori.Networking.WebSockets.Typed;
using osu.Framework.Bindables;

namespace fluXis.Online.Fluxel;

#nullable enable

public interface IAPIClient
{
    Bindable<APIUser?> User { get; }
    Bindable<ConnectionStatus> Status { get; }

    Bindable<UserActivity> Activity { get; }

    bool IsLoggedIn { get; }
    bool HasCredentials { get; }

    string AccessToken { get; }
    string MultifactorToken { get; set; }

    bool CanUseOnline { get; }
    EndpointConfig Endpoint { get; }
    Exception? LastException { get; }

    long LastReadTime { get; }
    Bindable<bool> HasUnreadNotifications { get; }
    List<APINotification> CurrentNotifications { get; }
    event Action<APINotification>? NewNotification;

    event Action<APIUser>? FriendOnline;
    event Action<APIUser>? FriendOffline;
    event Action<Achievement> AchievementEarned;
    event Action<ServerMessage> MessageReceived;
    event Action NameChangeRequested;

    event Action<string> ChatChannelAdded;
    event Action<string> ChatChannelRemoved;
    event Action<APIChatMessage> ChatMessageReceived;
    event Action<string, string> ChatMessageRemoved;

    event Action<string, List<CollectionItem>, List<CollectionItem>, List<string>>? OnCollectionUpdated;

    void PerformRequest(APIRequest request);
    Task PerformRequestAsync(APIRequest request);

    void PullServerConfig(Action complete, Action<Exception> failure);
    void TryConnecting();

    Task<Exception?> Login(string username, string password);
    Task<Exception?> ReLogin();
    Task<Exception?> Register(string username, string password, string email);
    void Logout();

    void UpdateLastRead();

    TypedWebSocketClient<S, C> GetWebSocket<S, C>(C target, string path)
        where S : class where C : class;

    public void Disconnect();
}
