namespace fluXis.Game.Map;

public class ScrollVelocityInfo : TimedObject
{
    public float Multiplier { get; set; }

    public ScrollVelocityInfo Copy()
    {
        return new ScrollVelocityInfo
        {
            Time = Time,
            Multiplier = Multiplier
        };
    }
}
