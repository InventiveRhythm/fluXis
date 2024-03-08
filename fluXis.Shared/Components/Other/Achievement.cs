using Newtonsoft.Json;

namespace fluXis.Shared.Components.Other;

public class Achievement
{
    [JsonProperty("id")]
    public string ID { get; set; } = null!;

    [JsonProperty("level")]
    public int Level { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; } = null!;

    [JsonProperty("description")]
    public string Description { get; set; } = null!;
}
