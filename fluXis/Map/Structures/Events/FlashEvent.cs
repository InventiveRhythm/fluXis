using System.ComponentModel;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Map.Structures.Bases;
using fluXis.Utils.Attributes;
using Newtonsoft.Json;
using osu.Framework.Graphics;
using osuTK.Graphics;

namespace fluXis.Map.Structures.Events;

[Description("Overlays a solid color over the screen.")]
[Icon(FluXisIconType.Flash)]
public class FlashEvent : IMapEvent, IHasDuration, IHasEasing
{
    [JsonProperty("time")]
    public double Time { get; set; }

    [JsonProperty("lane")]
    public int Lane { get; set; }

    [JsonProperty("group", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string Group { get; set; }

    [JsonProperty("duration")]
    public double Duration { get; set; }

    [JsonProperty("background")]
    public bool InBackground { get; set; }

    [JsonProperty("ease")]
    public Easing Easing { get; set; } = Easing.None;

    [JsonProperty("start-color")]
    public Color4 StartColor { get; set; } = Color4.White;

    [JsonProperty("start-alpha")]
    public float StartOpacity { get; set; } = 1;

    [JsonProperty("end-color")]
    public Color4 EndColor { get; set; } = Color4.White;

    [JsonProperty("end-alpha")]
    public float EndOpacity { get; set; }
}
