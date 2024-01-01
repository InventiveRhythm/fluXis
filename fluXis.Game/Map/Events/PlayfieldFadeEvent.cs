using fluXis.Game.Map.Structures;
using fluXis.Game.Utils;

namespace fluXis.Game.Map.Events;

public class PlayfieldFadeEvent : TimedObject
{
    public float FadeTime { get; set; }
    public float Alpha { get; set; }

    public override string ToString()
    {
        return $"PlayfieldFade({Time.ToStringInvariant()},{FadeTime.ToStringInvariant()},{Alpha.ToStringInvariant()})";
    }
}
