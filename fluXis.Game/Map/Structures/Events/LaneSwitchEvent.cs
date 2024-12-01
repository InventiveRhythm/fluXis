using fluXis.Game.Map.Structures.Bases;
using Newtonsoft.Json;
using osu.Framework.Graphics;

namespace fluXis.Game.Map.Structures.Events;

public class LaneSwitchEvent : IMapEvent, IHasDuration, IHasEasing
{
    [JsonProperty("time")]
    public double Time { get; set; }

    [JsonProperty("count")]
    public int Count { get; set; } = 1;

    [JsonProperty("speed")]
    public double Duration { get; set; }

    [JsonProperty("easing")]
    public Easing Easing { get; set; } = Easing.OutQuint;

    // improved version that actually considers finger positions properly
    public static readonly bool[][][] SWITCH_VISIBILITY_V2 =
    {
        new[] // 2k
        {
            new[] { true, false } // 1k
        },
        new[] // 3k
        {
            new[] { false, true, false }, // 1k
            new[] { true, false, true } // 2k
        },
        new[] // 4k
        {
            new[] { false, true, false, false }, // 1k
            new[] { false, true, true, false }, // 2k
            new[] { true, true, true, false } // 3k
        },
        new[] // 5k
        {
            new[] { false, false, true, false, false }, // 1k
            new[] { false, true, false, true, false }, // 2k
            new[] { false, true, true, true, false }, // 3k
            new[] { true, true, false, true, true } // 4k
        },
        new[] // 6k
        {
            new[] { false, false, true, false, false, false }, // 1k
            new[] { false, false, true, true, false, false }, // 2k
            new[] { false, true, true, true, false, false }, // 3k
            new[] { false, true, true, true, true, false }, // 4k
            new[] { true, true, true, false, true, true } // 5k
        },
        new[] // 7k
        {
            new[] { false, false, false, true, false, false, false }, // 1k
            new[] { false, false, true, false, true, false, false }, // 2k
            new[] { false, false, true, true, true, false, false }, // 3k
            new[] { false, true, true, false, true, true, false }, // 4k
            new[] { false, true, true, true, true, true, false }, // 5k
            new[] { true, true, true, false, true, true, true } // 6k
        },
        new[] // 8k
        {
            new[] { false, false, false, true, false, false, false, false }, // 1k
            new[] { false, false, true, false, false, true, false, false }, // 2k
            new[] { false, false, true, true, true, false, false, false }, // 3k
            new[] { false, true, true, false, false, true, true, false }, // 4k
            new[] { false, true, true, true, false, true, true, false }, // 5k
            new[] { true, true, true, false, false, true, true, true }, // 6k
            new[] { true, true, true, true, false, true, true, true } // 7k
        },
        new[] // 9k
        {
            new[] { false, false, false, false, true, false, false, false, false }, // 1k
            new[] { false, false, false, true, false, true, false, false, false }, // 2k
            new[] { false, false, false, true, true, true, false, false, false }, // 3k
            new[] { false, false, true, true, false, true, true, false, false }, // 4k
            new[] { false, false, true, true, true, true, true, false, false }, // 5k
            new[] { false, true, true, true, false, true, true, true, false }, // 6k
            new[] { false, true, true, true, true, true, true, true, false }, // 7k
            new[] { true, true, true, true, false, true, true, true, true } // 8k
        },
        new[] // 10k
        {
            new[] { false, false, false, false, true, false, false, false, false, false }, // 1k
            new[] { false, false, false, true, false, false, true, false, false, false }, // 2k
            new[] { false, false, false, true, true, false, true, false, false, false }, // 3k
            new[] { false, false, true, true, false, false, true, true, false, false }, // 4k
            new[] { false, false, true, true, true, false, true, true, false, false }, // 5k
            new[] { false, true, true, true, false, false, true, true, true, false }, // 6k
            new[] { false, true, true, true, true, false, true, true, true, false }, // 7k
            new[] { true, true, true, true, false, false, true, true, true, true }, // 8k
            new[] { true, true, true, true, true, false, true, true, true, true } // 9k
        }
    };

    public static readonly bool[][][] SWITCH_VISIBILITY =
    {
        new[] // 2k
        {
            new[] { true, false } // 1k
        },
        new[] // 3k
        {
            new[] { false, true, false }, // 1k
            new[] { true, false, true } // 2k
        },
        new[] // 4k
        {
            new[] { false, true, false, false }, // 1k
            new[] { true, false, false, true }, // 2k
            new[] { true, true, false, true } // 3k
        },
        new[] // 5k
        {
            new[] { false, false, true, false, false }, // 1k
            new[] { false, true, false, true, false }, // 2k
            new[] { false, true, true, true, false }, // 3k
            new[] { true, true, false, true, true } // 4k
        },
        new[] // 6k
        {
            new[] { false, false, true, false, false, false }, // 1k
            new[] { false, true, false, false, true, false }, // 2k
            new[] { false, true, true, false, true, false }, // 3k
            new[] { false, true, true, true, true, false }, // 4k
            new[] { true, true, true, false, true, true } // 5k
        },
        new[] // 7k
        {
            new[] { false, false, false, true, false, false, false }, // 1k
            new[] { false, false, true, false, true, false, false }, // 2k
            new[] { false, false, true, true, true, false, false }, // 3k
            new[] { false, true, true, false, true, true, false }, // 4k
            new[] { false, true, true, true, true, true, false }, // 5k
            new[] { true, true, true, false, true, true, true } // 6k
        },
        new[] // 8k
        {
            new[] { false, false, false, true, false, false, false, false }, // 1k
            new[] { false, false, true, false, false, true, false, false }, // 2k
            new[] { false, false, true, true, false, true, false, false }, // 3k
            new[] { false, true, true, false, false, true, true, false }, // 4k
            new[] { false, true, true, true, false, true, true, false }, // 5k
            new[] { false, true, true, true, true, true, true, false }, // 6k
            new[] { true, true, true, true, false, true, true, true } // 7k
        },
        new[] // 9k
        {
            new[] { false, false, false, false, true, false, false, false, false }, // 1k
            new[] { false, false, true, false, false, false, true, false, false }, // 2k
            new[] { false, false, true, false, true, false, true, false, false }, // 3k
            new[] { false, true, true, false, false, false, true, true, false }, // 4k
            new[] { false, true, true, false, true, false, true, true, false }, // 5k
            new[] { false, true, true, true, false, true, true, true, false }, // 6k
            new[] { false, true, true, true, true, true, true, true, false }, // 7k
            new[] { true, true, true, true, false, true, true, true, true } // 8k
        },
        new[] // 10k
        {
            new[] { false, false, false, false, true, false, false, false, false, false }, // 1k
            new[] { false, false, true, false, false, false, false, true, false, false }, // 2k
            new[] { false, false, true, false, true, false, false, true, false, false }, // 3k
            new[] { false, true, true, false, false, false, false, true, true, false }, // 4k
            new[] { false, true, true, false, true, false, false, true, true, false }, // 5k
            new[] { false, true, true, false, true, true, false, true, true, false }, // 6k
            new[] { false, true, true, true, true, false, true, true, true, false }, // 7k
            new[] { false, true, true, true, true, true, true, true, true, false }, // 8k
            new[] { true, true, true, true, true, false, true, true, true, true } // 9k
        }
    };
}
