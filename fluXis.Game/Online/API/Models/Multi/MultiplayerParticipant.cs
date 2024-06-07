using fluXis.Shared.Components.Multi;
using fluXis.Shared.Components.Users;

namespace fluXis.Game.Online.API.Models.Multi;

public class MultiplayerParticipant : IMultiplayerParticipant
{
    public long ID { get; init; }
    public MultiplayerUserState State { get; set; }

    public APIUser User { get; set; } = null!;
}
