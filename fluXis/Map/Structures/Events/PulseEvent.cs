using System;
using System.ComponentModel;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Map.Structures.Bases;
using fluXis.Screens.Gameplay.Overlay.Effect;
using fluXis.Utils.Attributes;
using fluXis.Utils.Extensions;
using Newtonsoft.Json;
using osu.Framework.Graphics;

namespace fluXis.Map.Structures.Events;

[Description("A pulsating border around the screen.")]
[Icon(FluXisIconType.Pulse)]
public class PulseEvent : IMapEvent, IHasDuration, IHasEasing
{
    [JsonProperty("time")]
    public double Time { get; set; }

    [JsonProperty("lane")]
    public int Lane { get; set; }

    [JsonProperty("group", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string Group { get; set; }

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
