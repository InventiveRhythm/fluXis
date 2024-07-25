using System;
using fluXis.Shared.Scoring.Structs;

namespace fluXis.Game.Scoring.Processing.Health;

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
    }

    public override void Update()
    {
        base.Update();

        if (lastTime == 0)
        {
            lastTime = GameplayClock.CurrentTime;
            return;
        }

        if (Screen.Playfield.Manager.Break)
        {
            // assign the time so that it doesn't jump when the break ends
            lastTime = GameplayClock.CurrentTime;
            return;
        }

        var delta = GameplayClock.CurrentTime - lastTime;

        HealthDrainRate = Math.Clamp(HealthDrainRate, -1f, 2f);
        Health.Value -= HealthDrainRate * (delta / 1000f);
        HealthDrainRate += 0.00016f * Difficulty * delta;

        lastTime = GameplayClock.CurrentTime;

        if (Health.Value == 0)
            TriggerFailure();
    }
}
