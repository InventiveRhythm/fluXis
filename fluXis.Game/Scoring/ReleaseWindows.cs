using fluXis.Game.Scoring.Enums;

namespace fluXis.Game.Scoring;

/// <summary>
/// The hit windows for long note ends.
/// </summary>
public class ReleaseWindows : HitWindows
{
    public override Judgement Lowest => Judgement.Alright;

    protected override Timing[] Timings { get; } =
    {
        new(Judgement.Flawless, 40),
        new(Judgement.Perfect, 73),
        new(Judgement.Great, 103),
        new(Judgement.Alright, 127)
    };

    public ReleaseWindows(float rate)
        : base(rate)
    {
    }
}
