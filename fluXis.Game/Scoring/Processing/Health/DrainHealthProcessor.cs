using System;
using fluXis.Shared.Scoring.Enums;
using fluXis.Shared.Scoring.Structs;

namespace fluXis.Game.Scoring.Processing.Health;

public class DrainHealthProcessor : HealthProcessor
{
    public float HealthDrainRate { get; private set; }
    private float lastTime;

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
            lastTime = (float)GameplayClock.CurrentTime;
            return;
        }

        if (Screen.Playfield.Manager.Break)
        {
            // assign the time so that it doesn't jump when the break ends
            lastTime = (float)GameplayClock.CurrentTime;
            return;
        }

        var delta = (float)GameplayClock.CurrentTime - lastTime;

        HealthDrainRate = Math.Max(HealthDrainRate, -1f);
        Health.Value -= HealthDrainRate * (delta / 1000f);
        HealthDrainRate += 0.001f * delta;

        lastTime = (float)GameplayClock.CurrentTime;

        if (Health.Value == 0)
            TriggerFailure();
    }
}
