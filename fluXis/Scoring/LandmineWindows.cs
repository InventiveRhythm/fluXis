using fluXis.Scoring.Enums;

namespace fluXis.Scoring;

public class LandmineWindows : HitWindows
{
    public LandmineWindows(float difficulty, float rate)
        : base(difficulty, rate)
    {
    }

    protected override Timing[] CreateTimings(float difficulty, float multiplier) => new Timing[]
    {
        new(Judgement.Miss, difficulty, multiplier, 64, 49, 34) //matches "Perfect" judgement from regular HitWindows
        //"None" timing here, or add a dedicated "Ignored" judgement for landmines?
    };
}
