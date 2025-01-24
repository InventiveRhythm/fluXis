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
    public long RoomID { get; init; }

    [JsonProperty("settings")]
    public MultiplayerRoomSettings Settings { get; init; } = new();

    [JsonProperty("host")]
    public APIUser Host { get; set; }

    [JsonProperty("participants")]
    public List<MultiplayerParticipant> Participants { get; init; } = new();

    [JsonProperty("map")]
    public APIMap Map { get; set; } = null!;

    public List<ScoreInfo> Scores { get; set; } = new();
}
