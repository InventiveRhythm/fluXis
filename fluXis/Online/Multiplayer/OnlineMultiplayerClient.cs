using System.Threading.Tasks;
using fluXis.Online.API;
using fluXis.Online.API.Models.Multi;
using fluXis.Online.API.Models.Users;
using fluXis.Online.API.Packets.Multiplayer;
using fluXis.Online.Fluxel;
using fluXis.Scoring;
using Midori.Networking.WebSockets.Frame;
using Midori.Networking.WebSockets.Typed;
using osu.Framework.Allocation;

namespace fluXis.Online.Multiplayer;

public partial class OnlineMultiplayerClient : MultiplayerClient
{
    [Resolved]
    private IAPIClient api { get; set; }

    public override APIUser Player => api.User.Value;

    private TypedWebSocketClient<IMultiplayerServer, IMultiplayerClient> connection = null!;

    [BackgroundDependencyLoader]
    private void load()
    {
        connection = api.GetWebSocket<IMultiplayerServer, IMultiplayerClient>(this, "/multiplayer");
        connection.OnClose += TriggerDisconnect;
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);

        connection?.Close(WebSocketCloseCode.NormalClosure, "Disconnecting.");
        connection?.Dispose();
    }

    protected override async Task<MultiplayerRoom> CreateRoom(string name, long mapid, string hash)
    {
        var room = await connection.Server.CreateRoom(name, "", mapid, hash);
        return room;
    }

    protected override async Task<MultiplayerRoom> JoinRoom(long id, string password)
    {
        var res = await api.SendAndWait(MultiJoinPacket.CreateC2SInitialJoin(id, password));

        if (!res.Success)
            throw new APIException(res.Message);

        return res.Success ? res.Data.Room : null;
    }

    public override async Task LeaveRoom()
    {
        await connection.Server.LeaveRoom();
        Room = null;
    }

    public override async Task ChangeMap(long map, string hash)
        => await connection.Server.UpdateMap(map, hash);

    public override Task UpdateScore(int score)
    {
        api.SendPacketAsync(MultiScorePacket.CreateC2S(score));
        return Task.CompletedTask;
    }

    public override Task Finish(ScoreInfo score)
    {
        api.SendPacketAsync(MultiCompletePacket.CreateC2S(score));
        return Task.CompletedTask;
    }
}
