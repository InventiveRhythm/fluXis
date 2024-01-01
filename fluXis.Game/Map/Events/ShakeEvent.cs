using fluXis.Game.Map.Structures;
using fluXis.Game.Utils;

namespace fluXis.Game.Map.Events;

public class ShakeEvent : TimedObject
{
    public float Duration { get; set; }
    public float Magnitude { get; set; } = 10;

    public override string ToString()
    {
        return $"Shake({Time.ToStringInvariant()},{Duration.ToStringInvariant()},{Magnitude.ToStringInvariant()})";
    }
}
