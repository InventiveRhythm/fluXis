using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Database.Maps;
using fluXis.Game.Map;
using fluXis.Game.Mods;
using fluXis.Game.Scoring.Enums;
using fluXis.Game.Scoring.Structs;
using osu.Framework.Bindables;

namespace fluXis.Game.Scoring.Processing;

public class ScoreProcessor : JudgementDependant
{
    public event Action OnComboBreak;

    public HitWindows HitWindows { get; init; }
    public RealmMap Map { get; init; }
    public MapInfo MapInfo { get; init; }
    public List<IMod> Mods { get; init; }

    public BindableFloat Accuracy { get; } = new(100);
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

    private int totalNotes => Flawless + Perfect + Great + Alright + Okay + Miss;
    private float ratedNotes => Flawless + Perfect * 0.98f + Great * 0.65f + Alright * 0.25f + Okay * 0.1f;

    public override void AddResult(HitResult result) => Recalculate();
    public override void RevertResult(HitResult result) => Recalculate();

    public void Recalculate()
    {
        Accuracy.Value = totalNotes == 0 ? 100 : ratedNotes / totalNotes * 100;
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

    public ScoreInfo ToScoreInfo()
    {
        return new ScoreInfo
        {
            Accuracy = Accuracy.Value,
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
            MapID = Map.OnlineID,
            MapHash = MapInfo.Hash,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            Mods = Mods.Select(m => m.Acronym).ToList()
        };
    }
}
