#nullable enable
using System;
using System.Threading.Tasks;
using fluXis.Online;
using fluXis.Online.Activity;
using fluXis.Online.API;
using fluXis.Online.API.Models.Users;
using fluXis.Online.API.Packets;
using fluXis.Online.Fluxel;
using osu.Framework.Bindables;
using osu.Framework.Logging;

namespace fluXis.Tests;

public class TestAPIClient : IAPIClient
{
    public Bindable<APIUser?> User => new(new APIUser
    {
        ID = 1,
        Username = "testing",
        CountryCode = "TD"
    });

    public Bindable<ConnectionStatus> Status { get; } = new(ConnectionStatus.Online);

    public Bindable<UserActivity> Activity { get; } = new();

    public string AccessToken => "token";
    public string MultifactorToken { get; set; } = "mfa-token";
    public APIEndpointConfig Endpoint => new();
    public Exception? LastException { get; private set; }
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
        if (username == "testing" && password == "passwd")
        {
            Status.Value = ConnectionStatus.Online;
            User.Value = new APIUser
            {
                ID = 1,
                Username = "Local User",
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

        Logger.Log($"status: {Status.Value}");
    }

    public Task SendPacket<T>(T packet) where T : IPacket => Task.CompletedTask;
    public void SendPacketAsync<T>(T packet) where T : IPacket { }
    public Task<FluxelReply<T>> SendAndWait<T>(T packet, long timeout = 10000) where T : IPacket => Task.FromResult(new FluxelReply<T>());
    public void RegisterListener<T>(EventType id, Action<FluxelReply<T>> listener) where T : IPacket { }
    public void UnregisterListener<T>(EventType id, Action<FluxelReply<T>> listener) where T : IPacket { }
    public void Disconnect() { }
}
