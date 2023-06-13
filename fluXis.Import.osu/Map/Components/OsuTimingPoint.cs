using System;
using fluXis.Game.Map;

namespace fluXis.Import.osu.Map.Components;

public class OsuTimingPoint
{
    public float Time { get; set; }
    public float BeatLength { get; set; }
    public int Meter { get; set; }
    public int Inherited { get; set; }

    public bool IsScrollVelocity => Inherited == 0 || BeatLength < 0;

    public TimingPointInfo ToTimingPointInfo()
    {
        return new TimingPointInfo
        {
            Time = Time,
            BPM = 60000 / BeatLength,
            Signature = Meter
        };
    }

    public ScrollVelocityInfo ToScrollVelocityInfo()
    {
        return new ScrollVelocityInfo
        {
            Time = Time,
            Multiplier = (float)Math.Clamp(-100 / (double)BeatLength, 0.1f, 10)
        };
    }
}
