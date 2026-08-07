using System.ComponentModel;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Map.Structures.Bases;
using fluXis.Utils.Attributes;
using Newtonsoft.Json;

namespace fluXis.Map.Structures.Events;

[Description("Shakes the screen.")]
[Icon(FluXisIconType.Shake)]
public class ShakeEvent : IMapEvent, IHasDuration
{
    [JsonProperty("time")]
    public double Time { get; set; }

    [JsonProperty("lane")]
    public int Lane { get; set; }

    [JsonProperty("group", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string Group { get; set; }

    [JsonProperty("duration")]
    public double Duration { get; set; }

    [JsonProperty("magnitude")]
    public float Magnitude { get; set; } = 10;
}
