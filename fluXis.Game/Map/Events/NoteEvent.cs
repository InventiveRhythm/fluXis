using fluXis.Game.Map.Structures;
using Newtonsoft.Json;

namespace fluXis.Game.Map.Events;

public class NoteEvent : ITimedObject
{
    [JsonProperty("time")]
    public double Time { get; set; }

    [JsonProperty("content")]
    public string Content { get; set; }
}
