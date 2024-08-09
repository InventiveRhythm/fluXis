using fluXis.Game.Map.Structures.Bases;
using Newtonsoft.Json;

namespace fluXis.Game.Map.Structures.Events;

public class PulseEvent : IMapEvent
{
    [JsonProperty("time")]
    public double Time { get; set; }
}
