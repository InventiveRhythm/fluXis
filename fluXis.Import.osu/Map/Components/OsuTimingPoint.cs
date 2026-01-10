using System;
using fluXis.Map.Structures;

namespace fluXis.Import.osu.Map.Components;

public class OsuTimingPoint
{
    public float Time { get; init; }
    public float BeatLength { get; init; }
    public int Meter { get; init; }
    public OsuSampleSet SampleSet { get; init; }
    public int Inherited { get; init; }

    public bool IsScrollVelocity => Inherited == 0 || BeatLength < 0;

    // some gimmick maps might have absurdly high bpm. fluxis doesn't like that, so let's cap it at a reasonable 10 millions bpm.
    public float BPM => Math.Clamp(60000 / BeatLength, 1, 10000000);

    public double ScrollMultiplier => Math.Clamp(-100 / (double)BeatLength, 0.1f, 10);

    public TimingPoint ToTimingPointInfo()
    {
        return new TimingPoint
        {
            Time = Time,
            BPM = BPM,
            Signature = Meter
        };
    }

    public ScrollVelocity ToScrollVelocityInfo(float dominantBpm, float bpm)
    {
        float bpmMultiplier = bpm / dominantBpm;

        return new ScrollVelocity
        {
            Time = Time,
            Multiplier = ScrollMultiplier * bpmMultiplier
        };
    }
}
