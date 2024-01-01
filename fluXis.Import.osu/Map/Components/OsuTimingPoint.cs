using System;
using fluXis.Game.Map.Structures;

namespace fluXis.Import.osu.Map.Components;

public class OsuTimingPoint
{
    public float Time { get; init; }
    public float BeatLength { get; init; }
    public int Meter { get; init; }
    public int Inherited { get; init; }

    public bool IsScrollVelocity => Inherited == 0 || BeatLength < 0;

    public TimingPoint ToTimingPointInfo()
    {
        return new TimingPoint
        {
            Time = Time,
            BPM = 60000 / BeatLength,
            Signature = Meter
        };
    }

    public ScrollVelocity ToScrollVelocityInfo()
    {
        return new ScrollVelocity
        {
            Time = Time,
            Multiplier = (float)Math.Clamp(-100 / (double)BeatLength, 0.1f, 10)
        };
    }
}
