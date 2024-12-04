using Newtonsoft.Json;

namespace fluXis.Game.Online.API.Models.Other;

public class APIGradientColor
{
    [JsonProperty("color")]
    public string Color { get; set; } = "#ffffff";

    [JsonProperty("position")]
    public double Position { get; set; }
}
