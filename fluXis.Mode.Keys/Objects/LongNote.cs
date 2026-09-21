using fluXis.Map.Structures;
using fluXis.Map.Structures.Bases;

namespace fluXis.Mode.Keys.Objects;

public class LongNote : HitObject, IHasDuration
{
    public double Duration { get; set; }
}
