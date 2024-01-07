using Newtonsoft.Json;

namespace fluXis.Game.Online.API.Models.Other;

public class GradientColor
{
    [JsonProperty("color")]
    public string Color { get; set; }

    [JsonProperty("position")]
    public double Position { get; set; }
}
