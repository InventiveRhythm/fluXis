using fluXis.Game.Map.Structures;
using Newtonsoft.Json;

namespace fluXis.Game.Map.Events;

public class PulseEvent : ITimedObject
{
    [JsonProperty("time")]
    public float Time { get; set; }
}
