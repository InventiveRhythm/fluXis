using System.ComponentModel;
using fluXis.Map.Structures.Bases;
using Newtonsoft.Json;

namespace fluXis.Map.Structures.Events;

[Description("Zooms in and out to the beat of the song.")]
public class BeatPulseEvent : IMapEvent
{
    [JsonProperty("time")]
    public double Time { get; set; }

    [JsonProperty("lane")]
    public int Lane { get; set; }

    [JsonProperty("group", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string Group { get; set; }

    [JsonProperty("strength")]
    public float Strength { get; set; } = 1.05f;

    /// <summary>
    /// How much of the length should be used to zoom in. (in %)
    /// </summary>
    [JsonProperty("zoom")]
    public float ZoomIn { get; set; } = .25f;

    [JsonProperty("interval")]
    public float Interval { get; set; } = 1;
}
