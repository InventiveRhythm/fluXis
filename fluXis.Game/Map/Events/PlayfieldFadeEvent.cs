using fluXis.Game.Map.Structures;
using Newtonsoft.Json;

namespace fluXis.Game.Map.Events;

public class PlayfieldFadeEvent : TimedObject
{
    [JsonProperty("duration")]
    public float FadeTime { get; set; }

    [JsonProperty("alpha")]
    public float Alpha { get; set; }
}
