using System.Collections.Generic;
using fluXis.Map.Structures;
using fluXis.Scoring.Enums;

namespace fluXis.Modes.Gameplay.Objects;

public class TrackedHitObject
{
    public readonly HitObject Object;

    public Judgement Judgement = Judgement.None;
    public double? ClockTime;

    public bool Judged => Judgement > Judgement.None;
    public double? Difference => Object.Time - ClockTime;

    public List<TrackedHitObject> Nested { get; } = [];

    public TrackedHitObject(HitObject o)
    {
        Object = o;
    }
}
