using System;
using fluXis.Game.Screens.Gameplay;
using fluXis.Game.Screens.Gameplay.Audio;
using fluXis.Shared.Scoring.Enums;
using fluXis.Shared.Scoring.Structs;
using osu.Framework.Bindables;
using osu.Framework.Utils;

namespace fluXis.Game.Scoring.Processing.Health;

public class HealthProcessor : JudgementDependant
{
    protected virtual double DefaultHealth => 100f;
    protected virtual bool DefaultFailCondition => Health.Value <= Health.MinValue;
    protected virtual bool ClearHealthOnFail => true;

    public GameplayScreen Screen { get; set; }
    protected GameplayClock GameplayClock => Screen.GameplayClock;

    public bool CanFail { get; set; } = true;
    public Func<HitResult, bool> ExtraFailCondition { get; set; }

    /// <summary>
    /// This is only really used for the health bar cross.
    /// To actually check if the player has failed, use <see cref="Failed"/>.
    /// </summary>
    public bool FailedAlready { get; private set; }

    public BindableDouble Health { get; }
    public float SmoothHealth { get; private set; }

    public bool Failed { get; private set; }
    public Action OnFail { get; set; }
    public double FailTime { get; private set; }

    protected float Difficulty { get; }

    public HealthProcessor(float difficulty)
    {
        Difficulty = difficulty;
        Health = new BindableDouble(DefaultHealth) { MinValue = 0, MaxValue = 100 };
    }

    protected void TriggerFailure()
    {
        FailedAlready = true;

        if (Failed || !CanFail)
            return;

        Failed = true;
        FailTime = GameplayClock.CurrentTime;
        OnFail?.Invoke();

        if (ClearHealthOnFail)
            Health.Value = Health.MinValue;
    }

    public override void AddResult(HitResult result)
    {
        if (FailedAlready) return;

        Health.Value += GetHealthIncreaseFor(result, Difficulty);

        if (meetsFailCondition(result))
            TriggerFailure();
    }

    private bool meetsFailCondition(HitResult result)
    {
        if (DefaultFailCondition)
            return true;

        return ExtraFailCondition != null && ExtraFailCondition.Invoke(result);
    }

    /// <returns>True if gameplay should exit normally.</returns>
    public virtual bool OnComplete() => true;

    public virtual void Update()
    {
        SmoothHealth = (float)Interpolation.Lerp(Health.Value, SmoothHealth, Math.Exp(-0.012 * Screen.Clock.ElapsedFrameTime));
    }

    protected virtual float GetHealthIncreaseFor(HitResult result, float difficulty) => result.Judgement switch
    {
        Judgement.Miss => 0.625f * -difficulty,
        Judgement.Okay => 0.375f * -difficulty,
        Judgement.Alright => 0f,
        Judgement.Great => 0.225f - difficulty * 0.025f,
        Judgement.Perfect => 0.6f - difficulty * 0.05f,
        Judgement.Flawless => 1.1f - difficulty * 0.1f,
        _ => 0
    };
}
