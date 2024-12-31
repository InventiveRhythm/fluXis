using fluXis.Scoring.Enums;
using osu.Framework.Graphics.Containers;

namespace fluXis.Skinning.Bases.Judgements;

public abstract partial class AbstractJudgementText : CompositeDrawable
{
    protected Judgement Judgement { get; }
    protected bool IsLate { get; }

    protected bool ShouldShowEarlyLate => Judgement != Judgement.Miss && Judgement != Judgement.Flawless;

    protected AbstractJudgementText(Judgement judgement, bool isLate)
    {
        Judgement = judgement;
        IsLate = isLate;
    }

    public abstract void Animate(bool showEarlyLate);
}
