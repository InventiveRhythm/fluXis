using System;
using System.Linq;
using fluXis.Game.Online.API.Maps;
using fluXis.Game.Scoring;
using fluXis.Game.Scoring.Enums;
using Newtonsoft.Json;

namespace fluXis.Game.Online.API.Scores;

public class APIScore
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("user")]
    public int UserId { get; set; }

    [JsonProperty("map")]
    public APIMapShort Map { get; set; }

    [JsonProperty("time")]
    public long Time { get; set; }

    [JsonProperty("mode")]
    public int Mode { get; set; }

    [JsonProperty("mods")]
    public string Mods { get; set; } = "";

    [JsonProperty("pr")]
    public double PerformanceRating { get; set; }

    [JsonProperty("score")]
    public int TotalScore { get; set; }

    [JsonProperty("accuracy")]
    public float Accuracy { get; set; }

    [JsonProperty("grade")]
    public string Rank { get; set; }

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

    public ScoreInfo ToScoreInfo()
    {
        return new ScoreInfo
        {
            Accuracy = Accuracy,
            Rank = (ScoreRank)Enum.Parse(typeof(ScoreRank), Rank),
            Score = TotalScore,
            Combo = 0,
            MaxCombo = MaxCombo,
            Flawless = FlawlessCount,
            Perfect = PerfectCount,
            Great = GreatCount,
            Alright = AlrightCount,
            Okay = OkayCount,
            Miss = MissCount,
            HitResults = null,
            MapID = Map.Id,
            MapHash = null,
            Mods = Mods.Split(" ").ToList()
        };
    }
}
