using fluXis.Online.API.Models.Users;

namespace fluXis.Online.API.Models.Multi;

public class MultiplayerParticipant : IMultiplayerParticipant
{
    public long ID { get; init; }
    public MultiplayerUserState State { get; set; }

    public APIUser User { get; set; } = null!;
}
