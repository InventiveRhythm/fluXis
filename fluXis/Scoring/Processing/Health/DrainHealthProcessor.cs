using System;
using fluXis.Scoring.Structs;

namespace fluXis.Scoring.Processing.Health;

public class DrainHealthProcessor : HealthProcessor
{
    public double HealthDrainRate { get; private set; }
    private double lastTime;

    public DrainHealthProcessor(float difficulty)
        : base(difficulty)
    {
    }

    public override void AddResult(HitResult result)
    {
        HealthDrainRate -= GetHealthIncreaseFor(result, Difficulty);

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

        var d = Clock.CurrentTime - lastTime;

        HealthDrainRate = Math.Clamp(HealthDrainRate, -1f, 2f);
        Health.Value -= HealthDrainRate * (d / 1000f);
        HealthDrainRate += 0.00016f * Difficulty * d;

        lastTime = Clock.CurrentTime;

        if (Health.Value == 0)
            TriggerFailure();
    }
}
