#nullable enable
using System;
using System.Threading.Tasks;
using fluXis.Game.Online;
using fluXis.Game.Online.API;
using fluXis.Game.Online.Fluxel;
using fluXis.Shared.Components.Users;
using osu.Framework.Bindables;
using osu.Framework.Logging;

namespace fluXis.Game.Tests;

public class TestAPIClient : IAPIClient
{
    public Bindable<APIUser?> User => new(new APIUser
    {
        ID = 1,
        Username = "testing",
        CountryCode = "TD"
    });

    public Bindable<ConnectionStatus> Status { get; } = new(ConnectionStatus.Online);
    public string AccessToken => "token";
    public APIEndpointConfig Endpoint => new();
    public Exception? LastException { get; private set; }
    public bool IsLoggedIn => Status.Value == ConnectionStatus.Online;

    public Action<APIRequest>? HandleRequest;

    public void PerformRequest(APIRequest request)
    {
        HandleRequest?.Invoke(request);
    }

    public async Task PerformRequestAsync(APIRequest request)
    {
        HandleRequest?.Invoke(request);
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
            Status.Value = ConnectionStatus.Failing;
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
}
