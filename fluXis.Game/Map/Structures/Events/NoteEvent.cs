using fluXis.Game.Map.Structures.Bases;
using Newtonsoft.Json;

namespace fluXis.Game.Map.Structures.Events;

public class NoteEvent : IMapEvent
{
    [JsonProperty("time")]
    public double Time { get; set; }

    [JsonProperty("content")]
    public string Content { get; set; }
}
