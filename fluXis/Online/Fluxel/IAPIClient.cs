using System;
using System.Threading.Tasks;
using fluXis.Online.Activity;
using fluXis.Online.API;
using fluXis.Online.API.Models.Users;
using fluXis.Online.API.Packets;
using osu.Framework.Bindables;

namespace fluXis.Online.Fluxel;

#nullable enable

public interface IAPIClient
{
    Bindable<APIUser?> User { get; }
    Bindable<ConnectionStatus> Status { get; }

    Bindable<UserActivity> Activity { get; }

    bool IsLoggedIn { get; }

    string AccessToken { get; }
    string MultifactorToken { get; set; }

    APIEndpointConfig Endpoint { get; }
    Exception? LastException { get; }

    void PerformRequest(APIRequest request);
    Task PerformRequestAsync(APIRequest request);

    void Login(string username, string password);
    void Register(string username, string password, string email);
    void Logout();

    // websocket stuff
    public Task SendPacket<T>(T packet) where T : IPacket;
    public void SendPacketAsync<T>(T packet) where T : IPacket;
    public Task<FluxelReply<T>> SendAndWait<T>(T packet, long timeout = 10000) where T : IPacket;

    public void RegisterListener<T>(EventType id, Action<FluxelReply<T>> listener) where T : IPacket;
    public void UnregisterListener<T>(EventType id, Action<FluxelReply<T>> listener) where T : IPacket;

    public void Disconnect();
}
