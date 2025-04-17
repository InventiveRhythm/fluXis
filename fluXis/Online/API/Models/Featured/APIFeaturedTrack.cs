using Newtonsoft.Json;

namespace fluXis.Online.API.Models.Featured;

public class APIFeaturedTrack
{
    [JsonProperty("id")]
    public string ID { get; set; } = null!;

    [JsonProperty("name")]
    public string Name { get; set; } = null!;

    [JsonProperty("length")]
    public string Length { get; set; } = null!;

    [JsonProperty("bpm")]
    public string BPM { get; set; } = null!;

    [JsonProperty("genre")]
    public string Genre { get; set; } = null!;
}
