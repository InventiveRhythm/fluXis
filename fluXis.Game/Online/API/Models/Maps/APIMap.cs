using fluXis.Game.Online.API.Models.Users;
using Newtonsoft.Json;

namespace fluXis.Game.Online.API.Models.Maps;

public class APIMap
{
    [JsonProperty("id")]
    public int Id { get; init; }

    [JsonProperty("mapset")]
    public int SetId { get; init; }

    [JsonProperty("hash")]
    public string Hash { get; init; } = "";

    [JsonProperty("mapper")]
    public APIUserShort Mapper { get; init; }

    [JsonProperty("difficulty")]
    public string Difficulty { get; init; } = "";

    [JsonProperty("mode")]
    public int KeyMode { get; init; }

    [JsonProperty("status")]
    public int Status { get; set; }

    [JsonProperty("title")]
    public string Title { get; init; } = "";

    [JsonProperty("artist")]
    public string Artist { get; init; } = "";

    [JsonProperty("source")]
    public string Source { get; init; } = "";

    [JsonProperty("tags")]
    public string Tags { get; init; } = "";

    [JsonProperty("bpm")]
    public double Bpm { get; init; }

    [JsonProperty("length")]
    public int Length { get; init; }

    [JsonProperty("rating")]
    public double Rating { get; init; }

    [JsonProperty("hits")]
    public int Hits { get; init; }

    [JsonProperty("lns")]
    public int LongNotes { get; init; }
}
