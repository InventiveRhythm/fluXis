using fluXis.Map;
using fluXis.Scoring.Structs;

namespace fluXis.Scoring.Processing;

public abstract class JudgementDependant
{
    public JudgementProcessor JudgementProcessor { get; set; }
    public virtual void ApplyMap(MapInfo map) { }
    public virtual void AddResult(HitResult result) { }
    public virtual void RevertResult(HitResult result) { }
}
