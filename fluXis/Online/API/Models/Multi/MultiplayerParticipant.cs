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

    //TODO: make it possible to select someone else in the room to play dual maps?

    public long ID => Player.ID;
}
