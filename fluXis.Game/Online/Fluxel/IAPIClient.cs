using System;
using System.Threading.Tasks;
using fluXis.Game.Online.API;
using fluXis.Shared.Components.Users;
using osu.Framework.Bindables;

namespace fluXis.Game.Online.Fluxel;

#nullable enable

public interface IAPIClient
{
    Bindable<APIUser?> User { get; }
    Bindable<ConnectionStatus> Status { get; }

    bool IsLoggedIn { get; }

    string AccessToken { get; }
    APIEndpointConfig Endpoint { get; }
    Exception? LastException { get; }

    void PerformRequest(APIRequest request);
    Task PerformRequestAsync(APIRequest request);

    void Login(string username, string password);
    void Register(string username, string password, string email);
    void Logout();
}
