using System;
using fluXis.Map;
using fluXis.Scoring.Enums;
using fluXis.Scoring.Structs;

namespace fluXis.Scoring.Processing.Health;

public class DrainHealthProcessor : HealthProcessor
{
    public double HealthDrainRate { get; private set; }

    private float factor = .2f;
    private int maxCombo;
    private double endTime;
    private double rate = 0;

    private double lastTime;

    public DrainHealthProcessor(float difficulty)
        : base(difficulty)
    {
    }

    public override void ApplyMap(MapInfo map, int playerIndex)
    {
        base.ApplyMap(map, playerIndex);
        maxCombo = map.MaxComboForPlayer(playerIndex);
        endTime = map.EndTime;
        factor = 0.008f + 80f / maxCombo;
    }

    public override void AddResult(HitResult result)
    {
        rate += GetHealthIncreaseFor(result, Difficulty);

        if (MeetsFailCondition(result))
            TriggerFailure();
    }

    public override void Update(double delta)
    {
        base.Update(delta);

        if (lastTime == 0)
        {
            lastTime = Clock.CurrentTime;
            return;
        }

        if (InBreak.Value)
        {
            // assign the time so that it doesn't jump when the break ends
            lastTime = Clock.CurrentTime;
            return;
        }

        delta = Clock.CurrentTime - lastTime;

        var loss = factor * maxCombo / (endTime / 1000);
        rate -= factor / (factor / .6f) * loss * (float)(delta / 1000f);

        if (!double.IsFinite(rate)) rate = 0;
        rate = Math.Clamp(rate, -3f, 2f);

        HealthDrainRate = -loss + rate;

        var change = HealthDrainRate * (delta / 1000f);
        Health.Value += !double.IsFinite(change) ? 0 : (float)change;

        lastTime = Clock.CurrentTime;

        if (Health.Value == 0)
            TriggerFailure();
    }

    protected override float GetHealthIncreaseFor(HitResult result, float difficulty)
    {
        return (float)(result.Judgement switch
        {
            Judgement.Flawless => 1.2f * Math.Log(2) * factor,
            Judgement.Perfect => .75f * Math.Log(2) * factor,
            Judgement.Great => .25f * Math.Log(2) * factor,
            Judgement.Alright => -1f * Math.Log(2) * factor,
            Judgement.Okay => -2 * Math.Log(2),
            Judgement.Miss => -5 * Math.Log(2),
            _ => throw new ArgumentOutOfRangeException(nameof(result), result, null)
        });
    }
}
