using Newtonsoft.Json;

namespace fluXis.Shared.Components.Other;

public class GradientColor
{
    [JsonProperty("color")]
    public string Color { get; set; } = "#ffffff";

    [JsonProperty("position")]
    public double Position { get; set; }
}
