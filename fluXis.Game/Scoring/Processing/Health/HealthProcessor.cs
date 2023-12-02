using System;
using fluXis.Game.Audio;
using fluXis.Game.Scoring.Enums;
using fluXis.Game.Scoring.Structs;
using fluXis.Game.Screens.Gameplay;
using osu.Framework.Bindables;

namespace fluXis.Game.Scoring.Processing.Health;

public class HealthProcessor : JudgementDependant
{
    protected virtual float DefaultHealth => 100f;
    protected virtual bool DefaultFailCondition => Health.Value <= Health.MinValue;
    protected virtual bool ClearHealthOnFail => true;

    public GameplayScreen Screen { get; set; }
    protected AudioClock AudioClock => Screen.AudioClock;

    public bool CanFail { get; set; } = true;
    public Func<HitResult, bool> ExtraFailCondition { get; set; }

    /// <summary>
    /// This is only really used for the health bar cross.
    /// To actually check if the player has failed, use <see cref="Failed"/>.
    /// </summary>
    public bool FailedAlready { get; private set; }

    public BindableFloat Health { get; }
    public bool Failed { get; private set; }
    public Action OnFail { get; set; }
    public double FailTime { get; private set; }

    public HealthProcessor()
    {
        Health = new BindableFloat(DefaultHealth) { MinValue = 0, MaxValue = 100 };
    }

    protected void TriggerFailure()
    {
        FailedAlready = true;

        if (Failed || !CanFail)
            return;

        Failed = true;
        FailTime = AudioClock.CurrentTime;
        OnFail?.Invoke();

        if (ClearHealthOnFail)
            Health.Value = Health.MinValue;
    }

    public override void AddResult(HitResult result)
    {
        if (FailedAlready) return;

        Health.Value += GetHealthIncreaseFor(result);

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

    public virtual void Update() { }

    protected virtual float GetHealthIncreaseFor(HitResult result)
    {
        return result.Judgement switch
        {
            Judgement.Miss => -5f,
            Judgement.Okay => -3f,
            Judgement.Alright => 0f,
            Judgement.Great => 0.025f,
            Judgement.Perfect => 0.2f,
            Judgement.Flawless => 0.3f,
            _ => 0f
        };
    }
}
