using System.Collections.Generic;
using fluXis.Online.API.Models.Maps;
using fluXis.Online.API.Models.Users;
using fluXis.Scoring;
using Newtonsoft.Json;

namespace fluXis.Online.API.Models.Multi;

[JsonObject(MemberSerialization.OptIn)]
public class MultiplayerRoom
{
    [JsonProperty("id")]
    public string RoomID { get; init; }

    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    [JsonProperty("privacy")]
    public MultiplayerPrivacy Privacy { get; init; }

    [JsonProperty("host")]
    public APIUser Host { get; set; }

    [JsonProperty("participants")]
    public List<MultiplayerParticipant> Participants { get; init; } = [];

    [JsonProperty("map")]
    public APIMap Map { get; set; } = null!;

    [JsonProperty("mods")]
    public string[] Mods { get; set; } = [];

    public List<ScoreInfo> Scores { get; set; } = new();
}
