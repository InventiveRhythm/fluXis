using fluXis.Online.API.Models.Users;
using Newtonsoft.Json;

namespace fluXis.Online.API.Models.Multi;

[JsonObject(MemberSerialization.OptIn)]
public class MultiplayerParticipant
{
    [JsonProperty("player")]
    public APIUser Player { get; init; }

    [JsonProperty("state")]
    public MultiplayerUserState State { get; set; }

    [JsonIgnore]
    public bool RequestingSkip { get; set; }

    public long ID => Player.ID;

    public static MultiplayerParticipant CreateDummy(MultiplayerUserState state = MultiplayerUserState.Idle) =>
        new MultiplayerParticipant
        {
            Player = APIUser.Dummy,
            State = state,
        };
}
