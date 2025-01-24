using System.Threading.Tasks;
using fluXis.Online.API.Models.Multi;
using fluXis.Scoring;

namespace fluXis.Online.Multiplayer;

public interface IMultiplayerServer
{
    Task<MultiplayerRoom> CreateRoom(string name, string password, long map, string hash);
    Task<MultiplayerRoom> JoinRoom(long id, string password);

    Task KickPlayer(long id);
    Task LeaveRoom();

    Task UpdateReadyState(bool ready);

    Task TransferHost(long id);

    Task<bool> UpdateMap(long map, string hash);

    Task UpdateScore(int score);
    Task FinishPlay(ScoreInfo score);
}
