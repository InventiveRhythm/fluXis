using System;
using fluXis.Game.Online.API.Models.Scores;
using fluXis.Game.Utils;
using Newtonsoft.Json;

namespace fluXis.Game.Online.API.Responses.Scores;

public class ScoreSubmissionStats
{
    [JsonProperty("score")]
    public APIScore Score { get; init; } = null!;

    [JsonProperty("ovr")]
    public StatisticChange OverallRating { get; init; } = null!;

    [JsonProperty("ptr")]
    public StatisticChange PotentialRating { get; init; } = null!;

    [JsonProperty("rank")]
    public StatisticChange Rank { get; init; } = null!;

    public ScoreSubmissionStats(APIScore score, double prevOvr, double prevPtr, int prevRank, double curOvr, double curPtr, int curRank)
    {
        Score = score;
        OverallRating = new StatisticChange(prevOvr, curOvr);
        PotentialRating = new StatisticChange(prevPtr, curPtr);
        Rank = new StatisticChange(prevRank, curRank);
    }

    [JsonConstructor]
    [Obsolete(JsonUtils.JSON_CONSTRUCTOR_ERROR, true)]
    public ScoreSubmissionStats()
    {
    }

    public class StatisticChange
    {
        [JsonProperty("prev")]
        public double Previous { get; init; }

        [JsonProperty("now")]
        public double Current { get; init; }

        public StatisticChange(double prev, double cur)
        {
            Previous = prev;
            Current = cur;
        }

        [JsonConstructor]
        [Obsolete(JsonUtils.JSON_CONSTRUCTOR_ERROR, true)]
        public StatisticChange()
        {
        }
    }
}
