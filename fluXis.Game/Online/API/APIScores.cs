using System.Collections.Generic;
using Newtonsoft.Json;

namespace fluXis.Game.Online.API;

public class APIScores
{
    [JsonProperty("scores")]
    public List<APIScore> Scores { get; set; } = new();

    [JsonProperty("map")]
    public APIMapShort Map { get; set; }
}
