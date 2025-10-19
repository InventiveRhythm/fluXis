using System;
using fluXis.Map.Structures.Bases;
using Newtonsoft.Json;
using osu.Framework.Graphics;

namespace fluXis.Map.Structures.Events.Camera;

public class CameraMoveEvent : ICameraEvent, IHasDuration, IHasEasing
{
    [JsonProperty("time")]
    public double Time { get; set; }

    [JsonProperty("x")]
    public float X { get; set; }

    [JsonProperty("y")]
    public float Y { get; set; }

    [JsonProperty("duration")]
    public double Duration { get; set; }

    [JsonProperty("ease")]
    public Easing Easing { get; set; }

    public void Apply(Drawable draw)
    {
        using (draw.BeginAbsoluteSequence(Time))
        {
            draw.MoveToX(X, Math.Max(Duration, 0), Easing);
            draw.MoveToY(Y, Math.Max(Duration, 0), Easing);
        }
    }
}
