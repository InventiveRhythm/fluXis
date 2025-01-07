using fluXis.Map.Structures.Bases;
using Newtonsoft.Json;
using osu.Framework.Graphics;

namespace fluXis.Map.Structures.Events;

public class HitObjectEaseEvent : IMapEvent, IHasEasing
{
    [JsonProperty("time")]
    public double Time { get; set; }

    [JsonProperty("ease")]
    public Easing Easing { get; set; }
}
