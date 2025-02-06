using System.Collections.Generic;
using System.Threading.Tasks;
using fluXis.Online.API.Models.Maps;
using fluXis.Online.API.Models.Multi;
using fluXis.Scoring;
using JetBrains.Annotations;

namespace fluXis.Online.Multiplayer;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public interface IMultiplayerClient
{
    Task UserJoined(MultiplayerParticipant participant);
    Task UserLeft(long id);

    Task UserStateChanged(long id, MultiplayerUserState state);

    Task MapUpdated(APIMap map);

    Task LoadRequested();
    Task ScoreUpdated(long user, int score);
    Task EveryoneFinished(List<ScoreInfo> scores);
}
