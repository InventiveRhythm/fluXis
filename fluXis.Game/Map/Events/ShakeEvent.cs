using fluXis.Game.Utils;

namespace fluXis.Game.Map.Events;

public class ShakeEvent : TimedObject
{
    public float Duration { get; set; }
    public float Magnitude { get; set; }

    public override string ToString()
    {
        return $"Shake({Time.ToStringInvariant()},{Duration.ToStringInvariant()},{Magnitude.ToStringInvariant()})";
    }
}
