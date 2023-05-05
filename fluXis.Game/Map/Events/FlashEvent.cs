using osu.Framework.Graphics;
using osuTK.Graphics;

namespace fluXis.Game.Map.Events;

public class FlashEvent : TimedObject
{
    public float Duration { get; set; }
    public bool InBackground { get; set; }
    public Easing Easing { get; set; }

    public Color4 StartColor { get; set; }
    public float StartOpacity { get; set; }

    public Color4 EndColor { get; set; }
    public float EndOpacity { get; set; }
}
