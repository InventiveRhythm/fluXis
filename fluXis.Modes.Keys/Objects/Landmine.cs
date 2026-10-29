using fluXis.Map.Structures;
using fluXis.Scoring.Enums;

namespace fluXis.Modes.Keys.Objects;

public class Landmine : HitObject
{
    public override float DensityContribution => 0;

    public bool Hidden { get; set; }

    public override bool ShouldShowJudgement(Judgement judgement) => judgement != Judgement.Flawless;
    public override string? GetSample(bool hit) => null;

    public override bool OnEditorMiddleClick()
    {
        Hidden = !Hidden;
        return true;
    }
}
