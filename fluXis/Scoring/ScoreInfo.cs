using System.Linq;
using System.Reflection;
using fluXis.Modes.Gameplay.Objects;
using fluXis.Online.API.Models.Users;
using fluXis.Scoring.Enums;
using Newtonsoft.Json;
using rhym;
using ResourceLocation = rhym.ResourceLocation;

namespace fluXis.Scoring;

#nullable enable

public class ScoreInfo
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

    [JsonProperty("results")]
    public Result[]? HitResults { get; set; }

    [JsonProperty("mapid")]
    public long MapID { get; set; }

    [JsonProperty("player")]
    public long PlayerID { get; set; }

    [JsonProperty("mods")]
    public string[] Mods { get; set; } = [];

    [JsonProperty("scrollspeed")]
    public float ScrollSpeed { get; set; }

    [JsonProperty("time")]
    public long Timestamp { get; set; }

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

    [JsonIgnore]
    public float Rate
    {
        get
        {
            var rate = Mods.FirstOrDefault(x => x.EndsWith('x')) ?? "1.0";
            rate = rate.TrimEnd('x');

            return float.TryParse(rate, out var result) ? result : 1;
        }
    }

    [JsonIgnore]
    public APIUser? Player { get; set; }

    public class Result
    {
        [JsonProperty("type")]
        public ResourceLocation ObjectType { get; init; }

        [JsonProperty("judgement")]
        public Judgement Judgement { get; init; }

        [JsonProperty("time")]
        public double Time { get; init; }

        [JsonProperty("delta")]
        public double Delta { get; init; }

        public Result(TrackedHitObject obj)
        {
            ObjectType = obj.Object.GetType().GetCustomAttribute<ResourceLocationAttribute>()?.Location ?? "flux:unknown";
            Judgement = obj.Judgement;
            Time = obj.ClockTime ?? obj.Object.Time;
            Delta = obj.Difference ?? 0;
        }

        public Result()
        {
        }
    }
}
