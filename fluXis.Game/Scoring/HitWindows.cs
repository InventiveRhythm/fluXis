using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Scoring.Enums;

namespace fluXis.Game.Scoring;

public class HitWindows
{
    public virtual Judgement HighestHitable => Judgement.Flawless;
    public virtual Judgement LowestHitable => Judgement.Okay;
    public virtual Judgement ComboBreakJudgement => Judgement.Miss;
    public virtual Judgement Lowest => Judgement.Miss;

    protected virtual Timing[] Timings { get; } =
    {
        new(Judgement.Flawless, 16),
        new(Judgement.Perfect, 40),
        new(Judgement.Great, 73),
        new(Judgement.Alright, 103),
        new(Judgement.Okay, 127),
        new(Judgement.Miss, 164)
    };

    public HitWindows(float multiplier = 1)
    {
        for (var i = 0; i < Timings.Length; ++i)
            Timings[i].Milliseconds *= multiplier;
    }

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

    public IEnumerable<Timing> GetTimings() => Timings;
}

public struct Timing
{
    public readonly Judgement Judgement;
    public float Milliseconds;

    public Timing(Judgement judgement, float milliseconds)
    {
        Judgement = judgement;
        Milliseconds = milliseconds;
    }
}
