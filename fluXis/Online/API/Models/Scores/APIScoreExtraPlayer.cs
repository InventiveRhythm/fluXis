using fluXis.Online.API.Models.Users;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace fluXis.Online.API.Models.Scores;

public class APIScoreExtraPlayer
{
    [JsonProperty("user")]
    public APIUser User { get; set; } = null!;

    [JsonProperty("pr")]
    public double PerformanceRating { get; set; }

    [JsonProperty("score")]
    public int TotalScore { get; set; }

    [JsonProperty("accuracy")]
    public float Accuracy { get; set; }

    [JsonProperty("grade")]
    public string Rank { get; set; } = "";

    [JsonProperty("maxcombo")]
    public int MaxCombo { get; set; }

    [JsonProperty("flawless")]
    public int FlawlessCount { get; set; }

    [JsonProperty("perfect")]
    public int PerfectCount { get; set; }

    [JsonProperty("great")]
    public int GreatCount { get; set; }

    [JsonProperty("alright")]
    public int AlrightCount { get; set; }

    [JsonProperty("okay")]
    public int OkayCount { get; set; }

    [JsonProperty("miss")]
    public int MissCount { get; set; }

    [JsonProperty("scrollspeed")]
    public float ScrollSpeed { get; set; }

    [JsonIgnore]
    [CanBeNull]
    public APIScore Score { get; set; }
}
