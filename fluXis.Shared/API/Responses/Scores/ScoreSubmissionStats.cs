using fluXis.Shared.Utils;
using Newtonsoft.Json;

namespace fluXis.Shared.API.Responses.Scores;

public class ScoreSubmissionStats
{
    [JsonProperty("id")]
    public long ID { get; set; }

    [JsonProperty("ovr")]
    public double OverallRating { get; init; }

    [JsonProperty("ptr")]
    public double PotentialRating { get; init; }

    [JsonProperty("rank")]
    public int Rank { get; init; }

    [JsonProperty("ovrChange")]
    public double OverallRatingChange { get; init; }

    [JsonProperty("ptrChange")]
    public double PotentialRatingChange { get; init; }

    [JsonProperty("rankChange")]
    public int RankChange { get; init; }

    public ScoreSubmissionStats(long id, double overallRating, double potentialRating, int rank, double overallRatingChange, double potentialRatingChange, int rankChange)
    {
        ID = id;
        OverallRating = overallRating;
        PotentialRating = potentialRating;
        Rank = rank;
        OverallRatingChange = overallRatingChange;
        PotentialRatingChange = potentialRatingChange;
        RankChange = rankChange;
    }

    [JsonConstructor]
    [Obsolete(JsonUtils.JSON_CONSTRUCTOR_ERROR, true)]
    public ScoreSubmissionStats()
    {
    }
}
