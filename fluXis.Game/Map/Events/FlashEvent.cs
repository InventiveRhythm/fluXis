using fluXis.Game.Utils;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osuTK.Graphics;

namespace fluXis.Game.Map.Events;

public class FlashEvent : TimedObject
{
    public float Duration { get; set; }
    public bool InBackground { get; set; }
    public Easing Easing { get; set; } = Easing.None;

    public Color4 StartColor { get; set; } = Color4.White;
    public float StartOpacity { get; set; } = 1;

    public Color4 EndColor { get; set; } = Color4.White;
    public float EndOpacity { get; set; }

    public override string ToString() => $"Flash({Time.ToStringInvariant()},{Duration.ToStringInvariant()},{InBackground.ToString()},{Easing.ToString()},{StartColor.ToHex()},{StartOpacity.ToStringInvariant()},{EndColor.ToHex()},{EndOpacity.ToStringInvariant()})";
}
