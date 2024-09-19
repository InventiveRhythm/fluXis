using fluXis.Game.Map.Structures.Bases;
using Newtonsoft.Json;
using osu.Framework.Graphics;

namespace fluXis.Game.Map.Structures.Events;

public class HitObjectEaseEvent : IMapEvent, IHasEasing
{
    [JsonProperty("time")]
    public double Time { get; set; }

    [JsonProperty("ease")]
    public Easing Easing { get; set; }
}
