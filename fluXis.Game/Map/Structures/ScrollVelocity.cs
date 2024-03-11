using Newtonsoft.Json;

namespace fluXis.Game.Map.Structures;

public class ScrollVelocity : ITimedObject
{
    [JsonProperty("time")]
    public float Time { get; set; }

    [JsonProperty("multiplier")]
    public float Multiplier { get; set; }

    public ScrollVelocity Copy()
    {
        return new ScrollVelocity
        {
            Time = Time,
            Multiplier = Multiplier
        };
    }
}
