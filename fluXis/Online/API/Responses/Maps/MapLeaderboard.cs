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
    [JsonProperty("map")]
    public APIMap Map { get; set; } = null!;

    [JsonProperty("scores")]
    public List<APIScore> Scores { get; set; } = new();

    public MapLeaderboard(APIMap map, IEnumerable<APIScore> scores)
    {
        Map = map;
        Scores = scores.ToList();
    }

    [JsonConstructor]
    [Obsolete(JsonUtils.JSON_CONSTRUCTOR_ERROR, true)]
    public MapLeaderboard()
    {
    }
}
