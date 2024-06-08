using System.Threading.Tasks;
using fluXis.Game.Online.API.Models.Multi;
using fluXis.Game.Online.Fluxel;
using fluXis.Shared.API;
using fluXis.Shared.API.Packets.Multiplayer;
using fluXis.Shared.Components.Users;
using fluXis.Shared.Scoring;
using osu.Framework.Allocation;

namespace fluXis.Game.Online.Multiplayer;

public partial class OnlineMultiplayerClient : MultiplayerClient
{
    [Resolved]
    private FluxelClient fluxel { get; set; }

    public override APIUser Player => fluxel.User.Value;
    private IMultiplayerClient impl => this;

    [BackgroundDependencyLoader]
    private void load()
    {
        fluxel.RegisterListener<MultiJoinPacket>(EventType.MultiplayerJoin, onUserJoined);
        fluxel.RegisterListener<MultiLeavePacket>(EventType.MultiplayerLeave, onUserLeave);
        fluxel.RegisterListener<MultiStatePacket>(EventType.MultiplayerState, onUserStateChange);
        fluxel.RegisterListener<MultiMapPacket>(EventType.MultiplayerMap, onMapChange);
        fluxel.RegisterListener<MultiStartPacket>(EventType.MultiplayerStartGame, onStartGame);
        fluxel.RegisterListener<MultiFinishPacket>(EventType.MultiplayerFinish, onGameFinished);
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);

        fluxel.UnregisterListener<MultiJoinPacket>(EventType.MultiplayerJoin, onUserJoined);
        fluxel.UnregisterListener<MultiLeavePacket>(EventType.MultiplayerLeave, onUserLeave);
        fluxel.UnregisterListener<MultiStatePacket>(EventType.MultiplayerState, onUserStateChange);
        fluxel.UnregisterListener<MultiMapPacket>(EventType.MultiplayerMap, onMapChange);
        fluxel.UnregisterListener<MultiStartPacket>(EventType.MultiplayerStartGame, onStartGame);
        fluxel.UnregisterListener<MultiFinishPacket>(EventType.MultiplayerFinish, onGameFinished);
    }

    private void onUserJoined(FluxelReply<MultiJoinPacket> reply) => impl.UserJoined(reply.Data.Participant as MultiplayerParticipant);
    private void onUserLeave(FluxelReply<MultiLeavePacket> reply) => impl.UserLeft(reply.Data.UserID);
    private void onUserStateChange(FluxelReply<MultiStatePacket> reply) => impl.UserStateChanged(reply.Data!.UserID, reply.Data.State);
    private void onMapChange(FluxelReply<MultiMapPacket> reply) => impl.MapChanged(reply.Success, reply.Data.Map, reply.Message);
    private void onStartGame(FluxelReply<MultiStartPacket> reply) => impl.Starting();
    private void onGameFinished(FluxelReply<MultiFinishPacket> reply) => impl.ResultsReady(reply.Data.Scores);

    protected override async Task<MultiplayerRoom> CreateRoom(string name, long mapid, string hash)
    {
        var res = await fluxel.SendAndWait(MultiCreatePacket.CreateC2S(name, "", mapid, hash));
        return res.Success ? res.Data.Room as MultiplayerRoom : null;
    }

    protected override async Task<MultiplayerRoom> JoinRoom(long id, string password)
    {
        var res = await fluxel.SendAndWait(MultiJoinPacket.CreateC2SInitialJoin(id, password));
        return res.Success ? res.Data.Room as MultiplayerRoom : null;
    }

    public override Task LeaveRoom()
    {
        fluxel.SendPacketAsync(new MultiLeavePacket());
        Room = null;
        return Task.CompletedTask;
    }

    public override Task ChangeMap(long map, string hash)
    {
        fluxel.SendPacketAsync(MultiMapPacket.CreateC2S(map, hash));
        return Task.CompletedTask;
    }

    public override Task Finish(ScoreInfo score)
    {
        fluxel.SendPacketAsync(MultiCompletePacket.CreateC2S(score));
        return Task.CompletedTask;
    }
}
