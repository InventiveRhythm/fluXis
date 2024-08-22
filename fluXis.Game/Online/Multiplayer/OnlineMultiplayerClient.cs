using System.Threading.Tasks;
using fluXis.Game.Online.API;
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
    private IAPIClient api { get; set; }

    public override APIUser Player => api.User.Value;

    [BackgroundDependencyLoader]
    private void load()
    {
        api.RegisterListener<MultiJoinPacket>(EventType.MultiplayerJoin, onUserJoined);
        api.RegisterListener<MultiLeavePacket>(EventType.MultiplayerLeave, onUserLeave);
        api.RegisterListener<MultiStatePacket>(EventType.MultiplayerState, onUserStateChange);
        api.RegisterListener<MultiMapPacket>(EventType.MultiplayerMap, onMapChange);
        api.RegisterListener<MultiStartPacket>(EventType.MultiplayerStartGame, onStartGame);
        api.RegisterListener<MultiFinishPacket>(EventType.MultiplayerFinish, onGameFinished);
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);

        api.UnregisterListener<MultiJoinPacket>(EventType.MultiplayerJoin, onUserJoined);
        api.UnregisterListener<MultiLeavePacket>(EventType.MultiplayerLeave, onUserLeave);
        api.UnregisterListener<MultiStatePacket>(EventType.MultiplayerState, onUserStateChange);
        api.UnregisterListener<MultiMapPacket>(EventType.MultiplayerMap, onMapChange);
        api.UnregisterListener<MultiStartPacket>(EventType.MultiplayerStartGame, onStartGame);
        api.UnregisterListener<MultiFinishPacket>(EventType.MultiplayerFinish, onGameFinished);
    }

    private void onUserJoined(FluxelReply<MultiJoinPacket> reply) => UserJoined(reply.Data.Participant as MultiplayerParticipant);
    private void onUserLeave(FluxelReply<MultiLeavePacket> reply) => UserLeft(reply.Data.UserID);
    private void onUserStateChange(FluxelReply<MultiStatePacket> reply) => UserStateChanged(reply.Data!.UserID, reply.Data.State);
    private void onMapChange(FluxelReply<MultiMapPacket> reply) => MapChanged(reply.Success, reply.Data.Map, reply.Message);
    private void onStartGame(FluxelReply<MultiStartPacket> reply) => Starting();
    private void onGameFinished(FluxelReply<MultiFinishPacket> reply) => ResultsReady(reply.Data.Scores);

    protected override async Task<MultiplayerRoom> CreateRoom(string name, long mapid, string hash)
    {
        var res = await api.SendAndWait(MultiCreatePacket.CreateC2S(name, "", mapid, hash));
        return res.Success ? res.Data.Room as MultiplayerRoom : null;
    }

    protected override async Task<MultiplayerRoom> JoinRoom(long id, string password)
    {
        var res = await api.SendAndWait(MultiJoinPacket.CreateC2SInitialJoin(id, password));

        if (!res.Success)
            throw new APIException(res.Message);

        return res.Success ? res.Data.Room as MultiplayerRoom : null;
    }

    public override Task LeaveRoom()
    {
        api.SendPacketAsync(new MultiLeavePacket());
        Room = null;
        return Task.CompletedTask;
    }

    public override Task ChangeMap(long map, string hash)
    {
        api.SendPacketAsync(MultiMapPacket.CreateC2S(map, hash));
        return Task.CompletedTask;
    }

    public override Task Finish(ScoreInfo score)
    {
        api.SendPacketAsync(MultiCompletePacket.CreateC2S(score));
        return Task.CompletedTask;
    }
}
