using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Scoring.Enums;
using fluXis.Game.Utils;

namespace fluXis.Game.Scoring;

public class HitWindows
{
    public virtual Judgement HighestHitable => Judgement.Flawless;
    public virtual Judgement LowestHitable => Judgement.Okay;
    public virtual Judgement ComboBreakJudgement => Judgement.Miss;
    public virtual Judgement Lowest => Judgement.Miss;

    protected Timing[] Timings { get; }

    public HitWindows(float difficulty, float rate)
    {
        Timings = CreateTimings(difficulty, rate);
    }

    protected virtual Timing[] CreateTimings(float difficulty, float multiplier) => new Timing[]
    {
        new(Judgement.Flawless, difficulty, multiplier, 22, 19, 13),
        new(Judgement.Perfect, difficulty, multiplier, 64, 49, 34),
        new(Judgement.Great, difficulty, multiplier, 97, 82, 67),
        new(Judgement.Alright, difficulty, multiplier, 127, 112, 97),
        new(Judgement.Okay, difficulty, multiplier, 151, 136, 121),
        new(Judgement.Miss, difficulty, multiplier, 188, 173, 158)
    };

    public Judgement JudgementFor(double milliseconds)
    {
        milliseconds = Math.Abs(milliseconds);

        for (var result = Judgement.Flawless; result >= Judgement.Miss; --result)
        {
            if (milliseconds <= TimingFor(result))
                return result;
        }

        return Judgement.Miss;
    }

    public float TimingFor(Judgement judgement) => Timings.FirstOrDefault(x => x.Judgement == judgement).Milliseconds;

    public bool CanBeHit(double milliseconds)
    {
        milliseconds = Math.Abs(milliseconds);
        return milliseconds <= TimingFor(Lowest);
    }

    public IEnumerable<Timing> GetTimings() => Timings;
}

public struct Timing
{
    public Judgement Judgement { get; }
    public float Milliseconds { get; }

    public Timing(Judgement judgement, float difficulty, float multiplier, float min, float mid, float max)
    {
        Judgement = judgement;
        Milliseconds = MapUtils.GetDifficulty(difficulty, min, mid, max) * multiplier;
    }
}
