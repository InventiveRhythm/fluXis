using fluXis.Map.Structures;
using fluXis.Scoring.Enums;

namespace fluXis.Mode.Keys.Objects;

public class LongNoteTail : HitObject
{
    public override bool ShouldShowJudgement(Judgement judgement) => false;
    public override string? GetSample(bool hit) => null;
}
