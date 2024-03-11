using fluXis.Game.Map.Structures;
using Newtonsoft.Json;

namespace fluXis.Game.Map.Events;

public class ShakeEvent : ITimedObject
{
    [JsonProperty("time")]
    public float Time { get; set; }

    [JsonProperty("duration")]
    public float Duration { get; set; }

    [JsonProperty("magnitude")]
    public float Magnitude { get; set; } = 10;
}
