using System.Collections.Generic;
using fluXis.Online.API.Models.Maps;
using fluXis.Online.API.Models.Users;
using fluXis.Scoring;
using Newtonsoft.Json;

namespace fluXis.Online.API.Models.Multi;

[JsonObject(MemberSerialization.OptIn)]
public class MultiplayerRoom
{
    [JsonIgnore]
    public const float MIN_VOTE_SKIP_MAJORITY = 0.7f;

    [JsonProperty("id")]
    public long RoomID { get; init; }

    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("privacy")]
    public MultiplayerPrivacy Privacy { get; init; }

    [JsonProperty("host")]
    public APIUser Host { get; set; }

    [JsonProperty("participants")]
    public List<MultiplayerParticipant> Participants { get; init; } = new();

    [JsonProperty("map")]
    public APIMap Map { get; set; } = null!;

    [JsonProperty("mods")]
    public List<string> Mods { get; set; } = new();

    public List<ScoreInfo> Scores { get; set; } = new();
}
