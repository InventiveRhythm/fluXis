using System.Collections.Generic;
using System.Threading.Tasks;
using fluXis.Game.Online.API.Models.Multi;
using fluXis.Shared.Components.Maps;
using fluXis.Shared.Components.Multi;
using fluXis.Shared.Scoring;

namespace fluXis.Game.Online.Multiplayer;

public interface IMultiplayerClient
{
    Task UserJoined(MultiplayerParticipant participant);
    Task UserLeft(long id);
    Task UserStateChanged(long id, MultiplayerUserState state);
    Task SettingsChanged(MultiplayerRoom room);
    Task MapChanged(bool success, IAPIMapShort map, string error);
    Task Starting();
    Task ResultsReady(List<ScoreInfo> scores);
}
