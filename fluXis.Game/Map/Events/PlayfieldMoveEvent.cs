using osu.Framework.Graphics;

namespace fluXis.Game.Map.Events;

public class PlayfieldMoveEvent : TimedObject
{
    public float OffsetX { get; set; }
    public float Duration { get; set; }
    public Easing Easing { get; set; }
}
