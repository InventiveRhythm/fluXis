using Newtonsoft.Json;

namespace fluXis.Shared.Components.Users;

public class APIUserStatistics
{
    [JsonProperty("ovr")]
    public double OverallRating { get; set; }

    [JsonProperty("ptr")]
    public double PotentialRating { get; set; }

    [JsonProperty("ova")]
    public double OverallAccuracy { get; set; }

    [JsonProperty("combo")]
    public int MaxCombo { get; set; }

    [JsonProperty("score")]
    public long RankedScore { get; set; }

    [JsonProperty("global")]
    public int GlobalRank { get; set; }

    [JsonProperty("country")]
    public int CountryRank { get; set; }

    [JsonProperty("plays")]
    public int PlayCount { get; set; }

    [JsonProperty("notes")]
    public long TotalNotes { get; set; }
}
