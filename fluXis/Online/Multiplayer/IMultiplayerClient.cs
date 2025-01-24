using System.Threading.Tasks;
using fluXis.Online.API.Models.Multi;

namespace fluXis.Online.Multiplayer;

public interface IMultiplayerClient
{
    Task UserStateChanged(long id, MultiplayerUserState state);
}
