using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Online.API.Models.Maps;
using fluXis.Online.API.Models.Scores;
using fluXis.Utils;
using Newtonsoft.Json;

namespace fluXis.Online.API.Responses.Maps;

public class MapLeaderboardClubs
{
    [JsonProperty("map")]
    public APIMap Map { get; set; } = null!;

    [JsonProperty("scores")]
    public List<APIClubScore> Scores { get; set; } = new();

    public MapLeaderboardClubs(APIMap map, IEnumerable<APIClubScore> scores)
    {
        Map = map;
        Scores = scores.ToList();
    }

    [JsonConstructor]
    [Obsolete(JsonUtils.JSON_CONSTRUCTOR_ERROR, true)]
    public MapLeaderboardClubs()
    {
    }
}
