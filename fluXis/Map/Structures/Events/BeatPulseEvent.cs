using fluXis.Map.Structures.Bases;
using Newtonsoft.Json;

namespace fluXis.Map.Structures.Events;

public class BeatPulseEvent : IMapEvent
{
    [JsonProperty("time")]
    public double Time { get; set; }

    [JsonProperty("strength")]
    public float Strength { get; set; } = 1.05f;

    /// <summary>
    /// How much of the length should be used to zoom in. (in %)
    /// </summary>
    [JsonProperty("zoom")]
    public float ZoomIn { get; set; } = .25f;
}
