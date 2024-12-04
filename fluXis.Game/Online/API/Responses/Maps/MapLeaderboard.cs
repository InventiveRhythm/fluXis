using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Online.API.Models.Maps;
using fluXis.Game.Online.API.Models.Scores;
using fluXis.Game.Utils;
using Newtonsoft.Json;

namespace fluXis.Game.Online.API.Responses.Maps;

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
