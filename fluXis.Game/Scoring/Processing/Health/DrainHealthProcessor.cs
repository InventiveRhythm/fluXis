using System;
using fluXis.Shared.Scoring.Enums;
using fluXis.Shared.Scoring.Structs;

namespace fluXis.Game.Scoring.Processing.Health;

public class DrainHealthProcessor : HealthProcessor
{
    public double HealthDrainRate { get; private set; }
    private double lastTime;

    public override void AddResult(HitResult result)
    {
        HealthDrainRate -= GetHealthIncreaseFor(result);
    }

    protected override float GetHealthIncreaseFor(HitResult result)
    {
        return result.Judgement switch
        {
            Judgement.Miss => -4f,
            Judgement.Okay => -2f,
            Judgement.Alright => -.5f,
            Judgement.Great => -.25f,
            Judgement.Perfect => 0f,
            Judgement.Flawless => .25f,
            _ => 0f
        };
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

        HealthDrainRate = Math.Max(HealthDrainRate, -1f);
        Health.Value -= HealthDrainRate * (delta / 1000f);
        HealthDrainRate += 0.001f * delta;

        lastTime = GameplayClock.CurrentTime;

        if (Health.Value == 0)
            TriggerFailure();
    }
}
