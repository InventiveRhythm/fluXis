using fluXis.Map;
using fluXis.Modes.Gameplay.Objects;

namespace fluXis.Scoring.Processing;

public abstract class JudgementDependant
{
    public JudgementProcessor JudgementProcessor { get; set; }
    public virtual void ApplyMap(PlayableMap map) { }
    public virtual void AddResult(TrackedHitObject result) { }
    public virtual void RevertResult(TrackedHitObject result) { }
}
