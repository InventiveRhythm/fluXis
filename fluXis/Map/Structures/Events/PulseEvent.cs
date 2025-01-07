using fluXis.Map.Structures.Bases;
using Newtonsoft.Json;

namespace fluXis.Map.Structures.Events;

public class PulseEvent : IMapEvent
{
    [JsonProperty("time")]
    public double Time { get; set; }
}
