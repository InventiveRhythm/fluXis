using fluXis.Game.Utils;

namespace fluXis.Game.Map.Events;

public class PulseEvent : TimedObject
{
    public override string ToString() => $"Pulse({Time.ToStringInvariant()})";
}
