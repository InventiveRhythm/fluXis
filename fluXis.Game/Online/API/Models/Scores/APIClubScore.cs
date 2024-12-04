using fluXis.Game.Online.API.Models.Clubs;
using Newtonsoft.Json;

namespace fluXis.Game.Online.API.Models.Scores;

public class APIClubScore
{
    [JsonProperty("club")]
    public APIClub Club { get; set; } = null!;

    [JsonProperty("map")]
    public long MapID { get; set; }

    [JsonProperty("score")]
    public long TotalScore { get; set; }

    [JsonProperty("pr")]
    public double PerformanceRating { get; set; }

    [JsonProperty("accuracy")]
    public double Accuracy { get; set; }
}
