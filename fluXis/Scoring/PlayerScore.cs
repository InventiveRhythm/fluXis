using System.Collections.Generic;
using fluXis.Scoring.Enums;
using fluXis.Scoring.Structs;
using Newtonsoft.Json;

namespace fluXis.Scoring;

#nullable enable

public class PlayerScore
{
    [JsonProperty("accuracy")]
    public float Accuracy { get; set; }

    [JsonProperty("grade")]
    public ScoreRank Rank { get; set; }

    [JsonProperty("score")]
    public int Score { get; set; }

    [JsonProperty("pr")]
    public double PerformanceRating { get; set; }

    [JsonProperty("combo")]
    public int Combo { get; set; }

    [JsonProperty("maxCombo")]
    public int MaxCombo { get; set; }

    [JsonProperty("flawless")]
    public int Flawless { get; set; }

    [JsonProperty("perfect")]
    public int Perfect { get; set; }

    [JsonProperty("great")]
    public int Great { get; set; }

    [JsonProperty("alright")]
    public int Alright { get; set; }

    [JsonProperty("okay")]
    public int Okay { get; set; }

    [JsonProperty("miss")]
    public int Miss { get; set; }

    [JsonProperty("player")]
    public long PlayerID { get; set; }

    [JsonProperty("results")]
    public List<HitResult>? HitResults { get; set; } = new();

    [JsonProperty("scrollspeed")]
    public float ScrollSpeed { get; set; }

    [JsonIgnore]
    public long Ratio
    {
        get
        {
            var nonFlawless = Perfect + Great + Alright + Okay + Miss;

            if (nonFlawless == 0)
                return 0;

            return Flawless * 100 / nonFlawless;
        }
    }

    [JsonIgnore]
    public bool FullFlawless => Flawless == MaxCombo;

    [JsonIgnore]
    public bool FullCombo => Miss == 0;
}
