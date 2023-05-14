using System.Collections.Generic;
using fluXis.Game.Scoring;
using JetBrains.Annotations;
using Realms;

namespace fluXis.Game.Database.Score;

public class RealmJudgements : RealmObject
{
    public int Flawless { get; set; }
    public int Perfect { get; set; }
    public int Great { get; set; }
    public int Alright { get; set; }
    public int Okay { get; set; }
    public int Miss { get; set; }

    public RealmJudgements(Dictionary<Judgement, int> judgements)
    {
        Flawless = judgements.TryGetValue(Judgement.Flawless, out var flawless) ? flawless : 0;
        Perfect = judgements.TryGetValue(Judgement.Perfect, out var perfect) ? perfect : 0;
        Great = judgements.TryGetValue(Judgement.Great, out var great) ? great : 0;
        Alright = judgements.TryGetValue(Judgement.Alright, out var alright) ? alright : 0;
        Okay = judgements.TryGetValue(Judgement.Okay, out var okay) ? okay : 0;
        Miss = judgements.TryGetValue(Judgement.Miss, out var miss) ? miss : 0;
    }

    [UsedImplicitly]
    private RealmJudgements()
    {
    }
}
