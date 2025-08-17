using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Scoring.Enums;
using fluXis.Scoring.Structs;
using Newtonsoft.Json;

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
    public List<HitResult>? HitResults { get; set; } = new();

    [JsonProperty("mapid")]
    public long MapID { get; set; }

    [JsonProperty("player")]
    public long PlayerID { get; set; }

    [JsonProperty("mods")]
    public List<string> Mods { get; set; } = new();

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

    public static List<ScoreInfo> CreateDummyLeaderboard(int count)
    {
        var random = new Random();
        var dummyScores = new List<ScoreInfo>();
        
        var noMomImnotusingImod = new[] { "1.0x", "1.2x", "1.5x", "2.0x", "NF", "HD", "FL", "NSV", "PA" };
        var ranks = Enum.GetValues<ScoreRank>();

        for (int i = 0; i < count; i++)
        {
            var maxCombo = random.Next(100, 2000);
            var flawless = random.Next(0, maxCombo);
            var perfect = random.Next(0, maxCombo - flawless);
            var great = random.Next(0, maxCombo - flawless - perfect);
            var alright = random.Next(0, maxCombo - flawless - perfect - great);
            var okay = random.Next(0, maxCombo - flawless - perfect - great - alright);
            var miss = maxCombo - flawless - perfect - great - alright - okay;

            var combo = random.Next(0, maxCombo + 1);
            var accuracy = random.Next(70, 101) + (float)random.NextDouble();
            
            var score = new ScoreInfo
            {
                Accuracy = accuracy,
                Rank = ranks[random.Next(ranks.Length)],
                Score = random.Next(50000, 1000000),
                PerformanceRating = random.Next(800, 2500) + random.NextDouble(),
                Combo = combo,
                MaxCombo = maxCombo,
                Flawless = flawless,
                Perfect = perfect,
                Great = great,
                Alright = alright,
                Okay = okay,
                Miss = miss,
                MapID = -1,
                PlayerID = -1,
                Mods = generateRandomMods(random, noMomImnotusingImod),
                ScrollSpeed = (float)(random.NextDouble() * 3 + 1.5),
                Timestamp = DateTimeOffset.Now.AddDays(-random.Next(0, 365)).ToUnixTimeSeconds()
            };

            dummyScores.Add(score);
        }

        return dummyScores;
    }

    private static List<string> generateRandomMods(Random random, string[] availableMods)
    {
        var modCount = random.Next(0, 4);
        var selectedMods = new List<string>();
        
        for (int i = 0; i < modCount; i++)
        {
            var mod = availableMods[random.Next(availableMods.Length)];
            if (!selectedMods.Contains(mod))
                selectedMods.Add(mod);
        }
        
        return selectedMods;
    }
}