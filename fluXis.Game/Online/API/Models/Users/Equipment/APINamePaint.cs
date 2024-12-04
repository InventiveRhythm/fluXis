using System.Collections.Generic;
using fluXis.Game.Online.API.Models.Other;
using Newtonsoft.Json;

namespace fluXis.Game.Online.API.Models.Users.Equipment;

public class APINamePaint
{
    [JsonProperty("id")]
    public string ID { get; set; } = "";

    [JsonProperty("name")]
    public string Name { get; set; } = "";

    [JsonProperty("colors")]
    public List<GradientColor> Colors { get; set; } = new();
}
