using Newtonsoft.Json;

namespace fluXis.Shared.Components.Clubs;

public class APIClubStatistics
{
    [JsonProperty("ovr")]
    public double OverallRating { get; set; }
}
