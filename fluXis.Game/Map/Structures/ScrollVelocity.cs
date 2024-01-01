namespace fluXis.Game.Map.Structures;

public class ScrollVelocity : TimedObject
{
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
