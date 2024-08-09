using Newtonsoft.Json;

namespace fluXis.Game.Map.Structures.Bases;

public interface ITimedObject
{
    [JsonProperty("time")]
    double Time { get; set; }
}
