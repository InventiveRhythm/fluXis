using System.Threading.Tasks;
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

    public override APIUserShort Player => fluxel.LoggedInUser;
    private IMultiplayerClient impl => this;

    [BackgroundDependencyLoader]
    private void load()
    {
        fluxel.RegisterListener<MultiJoinPacket>(EventType.MultiplayerJoin, onUserJoined);
        fluxel.RegisterListener<MultiLeavePacket>(EventType.MultiplayerLeave, onUserLeave);
        fluxel.RegisterListener<MultiReadyPacket>(EventType.MultiplayerReady, onReadyUpdate);
        fluxel.RegisterListener<MultiStartPacket>(EventType.MultiplayerStartGame, onStartGame);
        fluxel.RegisterListener<MultiFinishPacket>(EventType.MultiplayerFinish, onGameFinished);
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);

        fluxel.UnregisterListener<MultiJoinPacket>(EventType.MultiplayerJoin, onUserJoined);
        fluxel.UnregisterListener<MultiLeavePacket>(EventType.MultiplayerLeave, onUserLeave);
        fluxel.UnregisterListener<MultiReadyPacket>(EventType.MultiplayerReady, onReadyUpdate);
        fluxel.UnregisterListener<MultiStartPacket>(EventType.MultiplayerStartGame, onStartGame);
        fluxel.UnregisterListener<MultiFinishPacket>(EventType.MultiplayerFinish, onGameFinished);
    }

    private void onUserJoined(FluxelReply<MultiJoinPacket> reply) => impl.UserJoined(reply.Data.Player);
    private void onUserLeave(FluxelReply<MultiLeavePacket> reply) => impl.UserLeft(reply.Data.UserID);
    private void onReadyUpdate(FluxelReply<MultiReadyPacket> reply) => impl.ReadyStateChanged(reply.Data.PlayerID, reply.Data.Ready);
    private void onStartGame(FluxelReply<MultiStartPacket> reply) => impl.Starting();
    private void onGameFinished(FluxelReply<MultiFinishPacket> reply) => impl.ResultsReady(reply.Data.Scores);

    public override Task Finished(ScoreInfo score)
    {
        fluxel.SendPacketAsync(MultiCompletePacket.CreateC2S(score));
        return Task.CompletedTask;
    }
}
