using Newtonsoft.Json;

namespace fluXis.Game.Map.Structures;

public interface ITimedObject
{
    [JsonProperty("time")]
    double Time { get; set; }
}
