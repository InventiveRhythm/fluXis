using System;
using fluXis.Online.API.Models.Scores;
using Midori.Utils;
using Newtonsoft.Json;

namespace fluXis.Online.API.Models.Users;

public class APIUserScores
{
    [JsonProperty("recent_scores")]
    public APIScore[] Recent { get; set; } = null!;

    [JsonProperty("best_scores")]
    public APIScore[] Best { get; set; } = null!;

    public APIUserScores(APIScore[] recent, APIScore[] best)
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
