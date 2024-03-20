using fluXis.Game.Online.API.Models.Users;
using fluXis.Shared.Components.Multi;

namespace fluXis.Game.Online.API.Models.Multi;

public class MultiplayerParticipant : IMultiplayerParticipant
{
    public long ID { get; init; }
    public MultiplayerUserState State { get; set; }

    public APIUser User { get; set; } = null!;

    public void Resolve()
    {
        if (User != null)
            return;

        User = UserCache.GetUser(ID);
    }
}
