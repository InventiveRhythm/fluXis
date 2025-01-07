using fluXis.Map.Structures.Bases;
using Newtonsoft.Json;

namespace fluXis.Map.Structures.Events;

public class NoteEvent : IMapEvent
{
    [JsonProperty("time")]
    public double Time { get; set; }

    [JsonProperty("content")]
    public string Content { get; set; }
}
