using System;
using fluXis.Map.Structures.Bases;
using Newtonsoft.Json;
using osu.Framework.Graphics;

namespace fluXis.Map.Structures.Events.Camera;

public class CameraScaleEvent : ICameraEvent, IHasDuration, IHasEasing
{
    [JsonProperty("time")]
    public double Time { get; set; }

    [JsonProperty("scale")]
    public float Scale { get; set; } = 1f;

    [JsonProperty("duration")]
    public double Duration { get; set; }

    [JsonProperty("ease")]
    public Easing Easing { get; set; }

    public void Apply(Drawable draw)
    {
        using (draw.BeginAbsoluteSequence(Time))
            draw.ScaleTo(Scale, Math.Max(Duration, 0), Easing);
    }
}
