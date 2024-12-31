using Newtonsoft.Json;

namespace fluXis.Map.Structures.Bases;

public interface ITimedObject
{
    [JsonProperty("time")]
    double Time { get; set; }
}
