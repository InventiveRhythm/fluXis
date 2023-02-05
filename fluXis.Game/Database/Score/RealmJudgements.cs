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

    public RealmJudgements(Dictionary<Judgements, int> judgements)
    {
        Flawless = judgements.ContainsKey(Judgements.Flawless) ? judgements[Judgements.Flawless] : 0;
        Perfect = judgements.ContainsKey(Judgements.Perfect) ? judgements[Judgements.Perfect] : 0;
        Great = judgements.ContainsKey(Judgements.Great) ? judgements[Judgements.Great] : 0;
        Alright = judgements.ContainsKey(Judgements.Alright) ? judgements[Judgements.Alright] : 0;
        Okay = judgements.ContainsKey(Judgements.Okay) ? judgements[Judgements.Okay] : 0;
        Miss = judgements.ContainsKey(Judgements.Miss) ? judgements[Judgements.Miss] : 0;
    }

    [UsedImplicitly]
    private RealmJudgements()
    {
    }
}
