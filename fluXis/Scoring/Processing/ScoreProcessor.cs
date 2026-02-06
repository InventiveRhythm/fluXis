using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using fluXis.Map;
using fluXis.Mods;
using fluXis.Online.API.Models.Users;
using fluXis.Scoring.Enums;
using fluXis.Scoring.Structs;
using osu.Framework.Bindables;

namespace fluXis.Scoring.Processing;

public class ScoreProcessor : JudgementDependant, IDisposable
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

    public override void AddResult(HitResult result) => recalc();
    public override void RevertResult(HitResult result) => recalc();

    private readonly Action<Action> schedule;
    private readonly bool asyncCalculations;
    private readonly object recalcLock = new { };
    private bool needsToRecalc;

    private bool disposed;

    public ScoreProcessor(Action<Action> schedule, bool asyncCalculations = false)
    {
        this.schedule = schedule;
        this.asyncCalculations = asyncCalculations;

        if (asyncCalculations)
        {
            Task.Run(() =>
            {
                while (!disposed)
                {
                    bool st;

                    lock (recalcLock)
                        st = needsToRecalc;

                    if (!st)
                    {
                        Thread.Sleep(2);
                        continue;
                    }

                    lock (recalcLock)
                        needsToRecalc = false;

                    try
                    {
                        Recalculate();
                    }
                    catch
                    {
                    }
                }
            });
        }
    }

    private void recalc()
    {
        if (asyncCalculations)
        {
            lock (recalcLock)
                needsToRecalc = true;

            return;
        }

        Recalculate();
    }

    public void Recalculate(bool instant = false)
    {
        float acc = 0;
        float pr = 0;

        JudgementProcessor.RunLocked(() =>
        {
            int total = totalNotes;
            float rated = ratedNotes;

            acc = total == 0 ? 100 : rated / total * 100;
            pr = (float)CalculatePerformance(mapRating, Accuracy.Value, Flawless, Perfect, Great, Alright, Okay, Miss, Mods);
        });

        var rank = acc switch
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
        int nowCombo = 0;

        JudgementProcessor.RunLocked(() =>
        {
            var lastMissIndex = JudgementProcessor.Results.FindLastIndex(x => x.Judgement <= HitWindows.ComboBreakJudgement);
            nowCombo = lastMissIndex == -1 ? JudgementProcessor.Results.Count : JudgementProcessor.Results.Count - lastMissIndex - 1;
        });

        var maxCombo = Math.Max(nowCombo, MaxCombo);
        var score = getScore(acc / 100, maxCombo);

        if (asyncCalculations && !instant)
            schedule.Invoke(set);
        else
            set();

        void set()
        {
            if (curCombo > nowCombo)
                OnComboBreak?.Invoke();

            Accuracy.Value = acc;
            PerformanceRating.Value = pr;
            Rank.Value = rank;
            Combo.Value = nowCombo;
            MaxCombo = maxCombo;
            Score = score;
        }
    }

    private int getScore(double acc, int maxCombo)
    {
        var scoreMultiplier = 1f + Mods.Sum(mod => mod.ScoreMultiplier - 1f);

        var maxScore = 1000000 * scoreMultiplier;
        var accBased = (int)(acc * (maxScore * .9f));
        var comboBased = (int)(maxCombo / (float)MapInfo.MaxCombo * (maxScore * .1f));
        return accBased + comboBased;
    }

    public ScoreInfo ToScoreInfo()
    {
        Recalculate(true);

        return new ScoreInfo
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
    }

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

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        disposed = true;
    }
}
