using fluXis.Game.Online.API.Models.Users;
using fluXis.Shared.Components.Maps;
using Newtonsoft.Json;

namespace fluXis.Game.Online.API.Models.Maps;

public class APIMapShort : IAPIMapShort
{
    [JsonProperty("id")]
    public long ID { get; init; }

    [JsonProperty("mapset")]
    public long MapSet { get; set; }

    [JsonProperty("hash")]
    public string Hash { get; set; } = "";

    [JsonProperty("title")]
    public string Title { get; set; } = "";

    [JsonProperty("artist")]
    public string Artist { get; set; } = "";

    [JsonProperty("difficulty")]
    public string Difficulty { get; set; } = "";

    [JsonProperty("mode")]
    public int Mode { get; set; }

    [JsonProperty("rating")]
    public double Rating { get; set; }

    [JsonProperty("status")]
    public int Status { get; set; }

    [JsonProperty("mapper")]
    public APIUserShort Mapper { get; set; }
}
