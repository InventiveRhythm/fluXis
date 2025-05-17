using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Online.API.Models.Maps;
using fluXis.Online.API.Models.Scores;
using fluXis.Utils;
using Newtonsoft.Json;

namespace fluXis.Online.API.Responses.Maps;

public class MapLeaderboard
{
    [JsonProperty("set")]
    public APIMapSet MapSet { get; set; } = null!;

    [JsonProperty("map")]
    public APIMap Map { get; set; } = null!;

    [JsonProperty("scores")]
    public List<APIScore> Scores { get; set; } = new();

    public MapLeaderboard(APIMapSet set, APIMap map, IEnumerable<APIScore> scores)
    {
        MapSet = set;
        Map = map;
        Scores = scores.ToList();
    }

    [JsonConstructor]
    [Obsolete(JsonUtils.JSON_CONSTRUCTOR_ERROR, true)]
    public MapLeaderboard()
    {
    }
}
