namespace fluXis.Game.Map.Events;

public class LaneSwitchEvent : TimedObject
{
    public int Count { get; set; }
    public float Speed { get; set; } = 250;

    public static readonly bool[][][] SWITCH_VISIBILITY =
    {
        new[] // 5k
        {
            new[] { true, true, false, true, true } // 4k
        },
        new[] // 6k
        {
            new[] { false, true, true, true, true, false }, // 4k
            new[] { true, true, true, false, true, true } // 5k
        },
        new[] // 7k
        {
            new[] { false, true, true, false, true, true, false }, // 4k
            new[] { false, true, true, true, true, true, false }, // 5k
            new[] { true, true, true, false, true, true, true } // 6k
        }
    };
}
