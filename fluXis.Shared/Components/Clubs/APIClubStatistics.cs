using Newtonsoft.Json;

namespace fluXis.Shared.Components.Clubs;

public class APIClubStatistics
{
    [JsonProperty("ovr")]
    public double OverallRating { get; set; }

    [JsonProperty("claims")]
    public long TotalClaims { get; set; }

    [JsonProperty("claim-percent")]
    public double ClaimPercentage { get; set; }
}
