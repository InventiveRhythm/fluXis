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
        Flawless = judgements.ContainsKey(Judgement.Flawless) ? judgements[Judgement.Flawless] : 0;
        Perfect = judgements.ContainsKey(Judgement.Perfect) ? judgements[Judgement.Perfect] : 0;
        Great = judgements.ContainsKey(Judgement.Great) ? judgements[Judgement.Great] : 0;
        Alright = judgements.ContainsKey(Judgement.Alright) ? judgements[Judgement.Alright] : 0;
        Okay = judgements.ContainsKey(Judgement.Okay) ? judgements[Judgement.Okay] : 0;
        Miss = judgements.ContainsKey(Judgement.Miss) ? judgements[Judgement.Miss] : 0;
    }

    [UsedImplicitly]
    private RealmJudgements()
    {
    }
}
