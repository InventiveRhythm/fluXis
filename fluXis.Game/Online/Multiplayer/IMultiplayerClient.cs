using System.Collections.Generic;
using System.Threading.Tasks;
using fluXis.Game.Online.API.Models.Multi;
using fluXis.Shared.Components.Maps;
using fluXis.Shared.Components.Users;
using fluXis.Shared.Scoring;

namespace fluXis.Game.Online.Multiplayer;

public interface IMultiplayerClient
{
    Task LeaveRoom();
    Task UserJoined(APIUserShort user);
    Task UserLeft(long id);
    Task SettingsChanged(MultiplayerRoom room);
    Task ChangeMap(long map, string hash);
    Task MapChanged(bool success, IAPIMapShort map, string error);
    Task ReadyStateChanged(long userId, bool isReady);
    Task Starting();
    Task Finished(ScoreInfo score);
    Task ResultsReady(List<ScoreInfo> scores);
}
