using System;
using System.Collections.Generic;
using fluXis.Game.Online.API.Models.Scores;
using fluXis.Game.Utils;
using Newtonsoft.Json;

namespace fluXis.Game.Online.API.Models.Users;

public class APIUserScores
{
    [JsonProperty("recent_scores")]
    public List<APIScore> Recent { get; set; } = null!;

    [JsonProperty("best_scores")]
    public List<APIScore> Best { get; set; } = null!;

    public APIUserScores(List<APIScore> recent, List<APIScore> best)
    {
        Recent = recent;
        Best = best;
    }

    [JsonConstructor]
    [Obsolete(JsonUtils.JSON_CONSTRUCTOR_ERROR, true)]
    public APIUserScores()
    {
    }
}
