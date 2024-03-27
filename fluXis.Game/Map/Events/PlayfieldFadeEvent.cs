using fluXis.Game.Map.Structures;
using Newtonsoft.Json;

namespace fluXis.Game.Map.Events;

public class PlayfieldFadeEvent : ITimedObject, IHasDuration
{
    [JsonProperty("time")]
    public float Time { get; set; }

    [JsonProperty("duration")]
    public float Duration { get; set; }

    [JsonProperty("alpha")]
    public float Alpha { get; set; }
}
