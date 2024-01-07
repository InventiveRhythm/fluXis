using System.Collections.Generic;
using fluXis.Game.Online.API.Models.Other;
using Newtonsoft.Json;

namespace fluXis.Game.Online.API.Models.Clubs;

public class APIClubShort
{
    [JsonProperty("id")]
    public int ID { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("tag")]
    public string Tag { get; set; }

    [JsonProperty("color")]
    public List<GradientColor> Colors { get; set; }
}
