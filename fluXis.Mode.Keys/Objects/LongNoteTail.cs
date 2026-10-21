using fluXis.Map.Structures;
using rhym;
using fluXis.Scoring.Enums;

namespace fluXis.Mode.Keys.Objects;

[ResourceLocation("flux:keys/tail")]
public class LongNoteTail : HitObject
{
    public override bool ShouldShowJudgement(Judgement judgement) => false;
    public override string? GetSample(bool hit) => null;
}
