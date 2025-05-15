using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Map;
using fluXis.Mods;
using fluXis.Online.API.Models.Users;
using fluXis.Scoring.Enums;
using fluXis.Scoring.Structs;
using osu.Framework.Bindables;

namespace fluXis.Scoring.Processing;

public class ScoreProcessor : JudgementDependant
{
    public event Action OnComboBreak;

    public APIUser Player { get; set; } = APIUser.Default;

    public HitWindows HitWindows { get; init; }
    public MapInfo MapInfo { get; init; }
    public List<IMod> Mods { get; init; }

    public BindableFloat Accuracy { get; } = new(100);
    public BindableFloat PerformanceRating { get; } = new();
    public Bindable<ScoreRank> Rank { get; } = new();
    public int Score { get; private set; }
    public BindableInt Combo { get; } = new();
    public int MaxCombo { get; private set; }

    public int Flawless => JudgementProcessor.Results.Count(x => x.Judgement == Judgement.Flawless);
    public int Perfect => JudgementProcessor.Results.Count(x => x.Judgement == Judgement.Perfect);
    public int Great => JudgementProcessor.Results.Count(x => x.Judgement == Judgement.Great);
    public int Alright => JudgementProcessor.Results.Count(x => x.Judgement == Judgement.Alright);
    public int Okay => JudgementProcessor.Results.Count(x => x.Judgement == Judgement.Okay);
    public int Miss => JudgementProcessor.Results.Count(x => x.Judgement == Judgement.Miss);

    public bool FullFlawless => Flawless == totalNotes && Miss == 0;
    public bool FullCombo => Combo.Value == totalNotes;

    private float mapRating => MapInfo.RealmEntry!.Rating;

    private int totalNotes => Flawless + Perfect + Great + Alright + Okay + Miss;
    private float ratedNotes => Flawless + Perfect * 0.98f + Great * 0.65f + Alright * 0.25f + Okay * 0.1f;

    public override void AddResult(HitResult result) => Recalculate();
    public override void RevertResult(HitResult result) => Recalculate();

    public void Recalculate()
    {
        Accuracy.Value = totalNotes == 0 ? 100 : ratedNotes / totalNotes * 100;
        PerformanceRating.Value = (float)CalculatePerformance(mapRating, Accuracy.Value, Flawless, Perfect, Great, Alright, Okay, Miss, Mods);
        Rank.Value = Accuracy.Value switch
        {
            100 => ScoreRank.X,
            >= 99 => ScoreRank.SS,
            >= 98 => ScoreRank.S,
            >= 95 => ScoreRank.AA,
            >= 90 => ScoreRank.A,
            >= 80 => ScoreRank.B,
            >= 70 => ScoreRank.C,
            _ => ScoreRank.D
        };

        var curCombo = Combo.Value == 0 ? 1 : Combo.Value;
        int lastMissIndex = JudgementProcessor.Results.FindLastIndex(x => x.Judgement <= HitWindows.ComboBreakJudgement);
        Combo.Value = lastMissIndex == -1 ? JudgementProcessor.Results.Count : JudgementProcessor.Results.Count - lastMissIndex - 1;
        MaxCombo = Math.Max(Combo.Value, MaxCombo);
        Score = getScore();

        if (curCombo > Combo.Value)
            OnComboBreak?.Invoke();
    }

    private int getScore()
    {
        var scoreMultiplier = 1f + Mods.Sum(mod => mod.ScoreMultiplier - 1f);

        var maxScore = 1000000 * scoreMultiplier;
        var accBased = (int)(ratedNotes / MapInfo.MaxCombo * (maxScore * .9f));
        var comboBased = (int)(MaxCombo / (float)MapInfo.MaxCombo * (maxScore * .1f));
        return accBased + comboBased;
    }

    public ScoreInfo ToScoreInfo() => new()
    {
        Accuracy = Accuracy.Value,
        PerformanceRating = PerformanceRating.Value,
        Rank = Rank.Value,
        Score = Score,
        Combo = Combo.Value,
        MaxCombo = MaxCombo,
        Flawless = Flawless,
        Perfect = Perfect,
        Great = Great,
        Alright = Alright,
        Okay = Okay,
        Miss = Miss,
        HitResults = JudgementProcessor.Results,
        MapID = MapInfo.RealmEntry!.OnlineID,
        PlayerID = Player.ID,
        Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
        Mods = Mods.Select(m => m.Acronym).ToList()
    };

    public static double CalculatePerformance(float rating, float accuracy, int flawless, int perfect, int great, int alright, int okay, int miss, List<IMod> mods)
    {
        var ratedNotes = flawless + perfect * 0.98f + great * 0.65f + alright * 0.25f + okay * 0.1f;

        var val = Math.Pow(6 * Math.Max(1, rating / .1f), 2) / 2500; // base curve
        val *= Math.Max(0, 5 * (accuracy / 100f) - 4); // accuracy
        val *= 1 + .1f * Math.Min(1, ratedNotes / 2500); // length
        val *= Math.Pow(.99, miss); // misses

        if (mods.Any(x => x is RateMod))
        {
            var rate = mods.OfType<RateMod>().First().Rate;

            // https://www.geogebra.org/calculator/fjxrbmdq
            if (rate < 1)
                rate = (float)Math.Pow(rate, 3);
            else
                rate = 1.5f * rate - .5f;

            val *= rate;
        }

        if (mods.Any(x => x is EasyMod))
            val *= 0.8;
        if (mods.Any(x => x is HardMod))
            val *= 1.05;
        if (mods.Any(x => x is NoFailMod))
            val *= 0.4;
        if (mods.Any(x => x is NoSvMod))
            val *= 0.6;
        if (mods.Any(x => x is NoLnMod))
            val *= 0.6;
        if (mods.Any(x => x is NoEventMod))
            val *= 0.4;

        return val;
    }
}
