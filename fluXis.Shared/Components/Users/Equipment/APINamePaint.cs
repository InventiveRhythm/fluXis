using fluXis.Shared.Components.Other;
using Newtonsoft.Json;

namespace fluXis.Shared.Components.Users.Equipment;

public class APINamePaint
{
    [JsonProperty("id")]
    public string ID { get; set; } = "";

    [JsonProperty("name")]
    public string Name { get; set; } = "";

    [JsonProperty("colors")]
    public List<GradientColor> Colors { get; set; } = new();
}
