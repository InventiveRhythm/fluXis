using Newtonsoft.Json;

namespace fluXis.Game.Online.API.Models.Maps;

public class APIMapLookup
{
    [JsonProperty("id")]
    public long ID { get; set; }

    [JsonProperty("set")]
    public long SetID { get; set; }

    [JsonProperty("creator")]
    public long CreatorID { get; set; }

    [JsonProperty("status")]
    public int Status { get; set; }

    [JsonProperty("submitted")]
    public long DateSubmitted { get; set; }

    [JsonProperty("ranked")]
    public long? DateRanked { get; set; }

    [JsonProperty("updated")]
    public long LastUpdated { get; set; }

    [JsonProperty("hash")]
    public string Hash { get; set; } = string.Empty;
}
