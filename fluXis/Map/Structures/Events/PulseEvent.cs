using System;
using fluXis.Map.Structures.Bases;
using fluXis.Screens.Gameplay.Overlay.Effect;
using fluXis.Utils.Extensions;
using Newtonsoft.Json;
using osu.Framework.Graphics;

namespace fluXis.Map.Structures.Events;

public class PulseEvent : IMapEvent, IHasDuration, IHasEasing
{
    [JsonProperty("time")]
    public double Time { get; set; }

    [JsonProperty("width")]
    public float Width { get; set; } = 32;

    [JsonProperty("duration")]
    public double Duration { get; set; }

    [JsonProperty("in-percent")]
    public float InPercent { get; set; }

    [JsonProperty("easing")]
    public Easing Easing { get; set; } = Easing.Out;

    public void Apply(PulseEffect effect)
    {
        using (effect.BeginAbsoluteSequence(Time))
        {
            var dur = Math.Max(Duration, 0);

            effect.BorderTo(Width, InPercent * dur, Easing).Then()
                  .BorderTo(0, (float)dur * (1 - InPercent), Easing);
        }
    }
}
