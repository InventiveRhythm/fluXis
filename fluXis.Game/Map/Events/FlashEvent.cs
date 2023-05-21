using System.Globalization;
using osu.Framework.Extensions.Color4Extensions;
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

    public override string ToString() => $"Flash({Time.ToString(CultureInfo.InvariantCulture)},{Duration.ToString(CultureInfo.InvariantCulture)},{InBackground.ToString()},{Easing.ToString()},{StartColor.ToHex()},{StartOpacity.ToString(CultureInfo.InvariantCulture)},{EndColor.ToHex()},{EndOpacity.ToString(CultureInfo.InvariantCulture)})";
}
