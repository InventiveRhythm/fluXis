using fluXis.Online.API.Models.Users;
using fluXis.Scoring;
using Newtonsoft.Json;

namespace fluXis.Online.API.Models.Multi;

[JsonObject(MemberSerialization.OptIn)]
public class MultiplayerParticipant
{
    [JsonProperty("id")]
    public long ID { get; init; }

    [JsonProperty("state")]
    public MultiplayerUserState State { get; set; }

    public APIUser User { get; set; } = null!;
    public ScoreInfo Score { get; set; } = null!;
}
