using fluXis.Game.Scoring.Enums;

namespace fluXis.Game.Scoring;

/// <summary>
/// The hit windows for long note ends.
/// </summary>
public class ReleaseWindows : HitWindows
{
    public override Judgement Lowest => Judgement.Alright;
    public override Judgement LowestHitable => Judgement.Alright;

    public ReleaseWindows(float difficulty, float rate)
        : base(difficulty, rate)
    {
    }

    protected override Timing[] CreateTimings(float difficulty, float multiplier) => new Timing[]
    {
        new(Judgement.Flawless, difficulty, multiplier, 64, 49, 34),
        new(Judgement.Perfect, difficulty, multiplier, 97, 82, 67),
        new(Judgement.Great, difficulty, multiplier, 127, 112, 97),
        new(Judgement.Alright, difficulty, multiplier, 151, 136, 121)
    };
}
