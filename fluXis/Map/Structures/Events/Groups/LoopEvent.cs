using fluXis.Map.Structures.Bases;
using Newtonsoft.Json;

namespace fluXis.Map.Structures.Events.Groups;

public class LoopEvent : IMapEvent
{
    [JsonProperty("time")]
    public double Time { get; set; }

    [JsonProperty("lane")]
    public int Lane { get; set; }

    [JsonProperty("target")]
    public string TargetGroup { get; set; }

    [JsonProperty("distance")]
    public double Distance { get; set; }

    [JsonProperty("count")]
    public int Count { get; set; }

    [JsonIgnore]
    string ITimedObject.Group { get; set; }
}
