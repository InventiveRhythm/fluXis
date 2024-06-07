using System.Collections.Generic;
using fluXis.Shared.Components.Maps;
using Newtonsoft.Json;

namespace fluXis.Game.Online.API.Models.Scores;

public class APIScores
{
    [JsonProperty("scores")]
    public List<APIScore> Scores { get; set; } = new();

    [JsonProperty("map")]
    public APIMap Map { get; set; }
}
