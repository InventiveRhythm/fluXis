using fluXis.Game.Map.Structures;
using Newtonsoft.Json;

namespace fluXis.Game.Map.Events;

public class BeatPulseEvent : ITimedObject
{
    [JsonProperty("time")]
    public float Time { get; set; }

    [JsonProperty("strength")]
    public float Strength { get; set; } = 1.05f;

    /// <summary>
    /// How much of the length should be used to zoom in. (in %)
    /// </summary>
    [JsonProperty("zoom")]
    public float ZoomIn { get; set; } = .25f;
}
