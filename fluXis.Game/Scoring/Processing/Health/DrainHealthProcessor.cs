using System;
using fluXis.Shared.Scoring.Structs;

namespace fluXis.Game.Scoring.Processing.Health;

public class DrainHealthProcessor : HealthProcessor
{
    public double HealthDrainRate { get; private set; }
    private double lastTime;
    private readonly float nps;

    public DrainHealthProcessor(float difficulty, float nps)
        : base(difficulty)
    {
        this.nps = nps;
    }

    public override void AddResult(HitResult result)
    {
        if (FailedAlready) return;

        // only decrease health on lower judgements
        Health.Value += Math.Min(GetHealthIncreaseFor(result, Difficulty), 0);

        if (MeetsFailCondition(result))
            TriggerFailure();

        HealthDrainRate -= GetHealthIncreaseFor(result, Difficulty) >= 0
            ? nps * 0.2 * GetHealthIncreaseFor(result, Difficulty) // reduce health drain (good judgements)
            : nps * 0.3 * GetHealthIncreaseFor(result, Difficulty); // increase health drain (bad judgements)
    }

    public override void Update()
    {
        base.Update();

        if (lastTime == 0)
        {
            lastTime = GameplayClock.CurrentTime;
            return;
        }

        if (Screen.PlayfieldManager.InBreak)
        {
            // assign the time so that it doesn't jump when the break ends
            lastTime = GameplayClock.CurrentTime;
            return;
        }

        var delta = GameplayClock.CurrentTime - lastTime;

        HealthDrainRate = Math.Clamp(HealthDrainRate, -5f, 5f);
        Health.Value -= HealthDrainRate * (delta / 1000f);
        HealthDrainRate += 0.000006f * nps * Difficulty * delta;

        lastTime = GameplayClock.CurrentTime;
    }
}
