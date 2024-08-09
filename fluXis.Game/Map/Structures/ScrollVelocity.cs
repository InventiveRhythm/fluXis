using fluXis.Game.Map.Structures.Bases;
using Newtonsoft.Json;

namespace fluXis.Game.Map.Structures;

public class ScrollVelocity : ITimedObject
{
    [JsonProperty("time")]
    public double Time { get; set; }

    [JsonProperty("multiplier")]
    public double Multiplier { get; set; }

    public ScrollVelocity Copy()
    {
        return new ScrollVelocity
        {
            Time = Time,
            Multiplier = Multiplier
        };
    }
}
