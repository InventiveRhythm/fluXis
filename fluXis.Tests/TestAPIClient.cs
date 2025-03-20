#nullable enable
using System;
using System.Threading.Tasks;
using fluXis.Online;
using fluXis.Online.Activity;
using fluXis.Online.API;
using fluXis.Online.API.Models.Chat;
using fluXis.Online.API.Models.Other;
using fluXis.Online.API.Models.Users;
using fluXis.Online.Fluxel;
using Midori.Networking.WebSockets.Typed;
using osu.Framework.Bindables;
using osu.Framework.Logging;

namespace fluXis.Tests;

public class TestAPIClient : IAPIClient
{
    public const string USERNAME = "testing";
    public const string PASSWORD = "passwd";

    public Bindable<APIUser?> User => new(new APIUser
    {
        ID = 1,
        Username = USERNAME,
        CountryCode = "TD"
    });

    public Bindable<ConnectionStatus> Status { get; } = new(ConnectionStatus.Online);

    public Bindable<UserActivity> Activity { get; } = new();

    public string AccessToken => "token";
    public string MultifactorToken { get; set; } = "mfa-token";
    public APIEndpointConfig Endpoint => new();
    public Exception? LastException { get; private set; }

    public event Action<APIUser>? FriendOnline;
    public event Action<APIUser>? FriendOffline;
    public event Action<Achievement>? AchievementEarned;
    public event Action<ServerMessage>? MessageReceived;
    public event Action? NameChangeRequested;
    public event Action<string>? ChatChannelAdded;
    public event Action<string>? ChatChannelRemoved;
    public event Action<APIChatMessage>? ChatMessageReceived;
    public event Action<string, string>? ChatMessageRemoved;

    public bool IsLoggedIn => Status.Value == ConnectionStatus.Online;

    public Action<APIRequest>? HandleRequest { get; set; }

    public void PerformRequest(APIRequest request)
    {
        Logger.Log($"Handling request {request.GetType()}.");
        HandleRequest?.Invoke(request);
    }

    public async Task PerformRequestAsync(APIRequest request)
    {
        PerformRequest(request);
        await Task.CompletedTask;
    }

    public void Login(string username, string password)
    {
        if (username == USERNAME && password == PASSWORD)
        {
            Status.Value = ConnectionStatus.Online;
            User.Value = new APIUser
            {
                ID = 1,
                Username = USERNAME,
                CountryCode = "TD"
            };
        }
        else
        {
            Status.Value = ConnectionStatus.Failed;
            LastException = new APIException("Invalid credentials");
            User.Value = null;
        }
    }

    public void Register(string username, string password, string email)
    {
        Status.Value = ConnectionStatus.Online;
        User.Value = new APIUser
        {
            ID = 1,
            Username = username,
            CountryCode = "TD"
        };
    }

    public void Logout()
    {
        Logger.Log("Logging out");
        Status.Value = ConnectionStatus.Offline;
        User.Value = null;
    }

    public TypedWebSocketClient<S, C> GetWebSocket<S, C>(C target, string path) where S : class where C : class
    {
        throw new NotImplementedException();
    }

    public void Disconnect() { }
}
