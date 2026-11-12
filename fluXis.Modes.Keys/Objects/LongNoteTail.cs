using fluXis.Map.Structures;
using fluXis.Scoring.Enums;
using rhym;

namespace fluXis.Modes.Keys.Objects;

[ResourceLocation("flux:keys/tail")]
public class LongNoteTail : HitObject
{
    public override bool ShouldShowJudgement(Judgement judgement) => false;
    public override string? GetSample(bool hit) => null;
}
