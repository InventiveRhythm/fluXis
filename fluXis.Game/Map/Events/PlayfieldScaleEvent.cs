using fluXis.Game.Map.Structures;
using fluXis.Game.Utils;
using osu.Framework.Graphics;

namespace fluXis.Game.Map.Events;

public class PlayfieldScaleEvent : TimedObject
{
    public float ScaleX { get; set; }
    public float ScaleY { get; set; }
    public float Duration { get; set; }
    public Easing Easing { get; set; }

    public override string ToString() => $"PlayfieldScale({Time.ToStringInvariant()},{ScaleX.ToStringInvariant()},{ScaleY.ToStringInvariant()},{Duration.ToStringInvariant()},{Easing.ToString()})";
}
