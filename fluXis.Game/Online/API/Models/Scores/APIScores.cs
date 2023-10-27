using System.Collections.Generic;
using fluXis.Game.Online.API.Models.Maps;
using Newtonsoft.Json;

namespace fluXis.Game.Online.API.Models.Scores;

public class APIScores
{
    [JsonProperty("scores")]
    public List<APIScore> Scores { get; set; } = new();

    [JsonProperty("map")]
    public APIMapShort Map { get; set; }
}
