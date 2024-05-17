using System;
using System.Threading.Tasks;
using fluXis.Game.Online.API;
using fluXis.Shared.Components.Users;
using osu.Framework.Bindables;

namespace fluXis.Game.Online.Fluxel;

public interface IAPIClient
{
    Bindable<APIUserShort> User { get; }
    Bindable<ConnectionStatus> Status { get; }

    string AccessToken { get; }
    APIEndpointConfig Endpoint { get; }
    Exception LastException { get; }

    void PerformRequest(APIRequest request);
    Task PerformRequestAsync(APIRequest request);
}
