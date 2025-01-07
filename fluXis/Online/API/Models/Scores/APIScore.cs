using fluXis.Online.API.Models.Maps;
using fluXis.Online.API.Models.Users;
using Newtonsoft.Json;

namespace fluXis.Online.API.Models.Scores;

#nullable enable

public class APIScore
{
    [JsonProperty("id")]
    public long ID { get; set; }

    [JsonProperty("user")]
    public APIUser User { get; set; } = null!;

    [JsonProperty("time")]
    public long Time { get; set; }

    [JsonProperty("mode")]
    public int Mode { get; set; }

    /// <summary>
    /// List of mods seperated by commas.
    /// </summary>
    [JsonProperty("mods")]
    public string Mods { get; set; } = "";

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

    #region Optional

    [JsonProperty("map")]
    public APIMap? Map { get; set; }

    #endregion
}
