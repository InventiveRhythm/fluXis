using Newtonsoft.Json;

namespace fluXis.Game.Online.API.Models.Scores;

public class APIScoreResponse
{
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
}
