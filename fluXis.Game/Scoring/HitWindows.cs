using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Scoring.Enums;

namespace fluXis.Game.Scoring;

// ReSharper disable MemberCanBeMadeStatic.Global
#pragma warning disable CA1822
public class HitWindows
{
    public Judgement HighestHitable => Judgement.Flawless;
    public Judgement LowestHitable => Judgement.Okay;
    public Judgement ComboBreakJudgement => Judgement.Miss;
    public Judgement Lowest => Judgement.Miss;

    private readonly Timing[] timings =
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
        for (var i = 0; i < timings.Length; ++i)
            timings[i].Milliseconds *= multiplier;
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

    public float TimingFor(Judgement judgement) => timings.FirstOrDefault(x => x.Judgement == judgement).Milliseconds;

    public IEnumerable<Timing> GetTimings() => timings;
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
