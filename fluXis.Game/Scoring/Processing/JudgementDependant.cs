using fluXis.Game.Map;
using fluXis.Shared.Scoring.Structs;

namespace fluXis.Game.Scoring.Processing;

public abstract class JudgementDependant
{
    public JudgementProcessor JudgementProcessor { get; set; }
    public virtual void ApplyMap(MapInfo map) { }
    public virtual void AddResult(HitResult result) { }
    public virtual void RevertResult(HitResult result) { }
}
