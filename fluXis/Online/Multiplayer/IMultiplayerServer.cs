using System.Collections.Generic;
using System.Threading.Tasks;
using fluXis.Online.API.Models.Multi;
using fluXis.Scoring;
using JetBrains.Annotations;

namespace fluXis.Online.Multiplayer;

[UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
public interface IMultiplayerServer
{
    Task<MultiplayerRoom> CreateRoom(string name, MultiplayerPrivacy privacy, string password, long map, string hash);
    Task<MultiplayerRoom> JoinRoom(long id, string password);

    Task KickPlayer(long id);
    Task LeaveRoom();

    Task UpdateReadyState(bool ready);

    Task TransferHost(long id);

    Task<bool> UpdateMap(long map, string hash, List<string> mods);

    Task UpdateScore(int score);
    Task FinishPlay(ScoreInfo score);
}
