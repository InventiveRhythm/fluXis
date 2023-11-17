using System;
using osu.Framework.Graphics;

namespace fluXis.Game.Utils;

/// <summary>
/// A class that tracks the time since the last user input
/// </summary>
public partial class IdleTracker : Component
{
    public double IdleTime { get; set; }
    public Action TriggerAction { get; set; }
    public Action RestartAction { get; set; }

    private bool alreadyTriggered = true;
    private double timeLeft;

    /// <summary>
    /// Creates a new IdleTracker
    /// </summary>
    /// <param name="idleTime">
    /// The time in milliseconds after which the trigger action should be called
    /// </param>
    /// <param name="triggerAction">
    /// The action to be called after the idle time has passed
    /// </param>
    /// <param name="restartAction">
    /// The action to be called when the tracker is reset
    /// </param>
    public IdleTracker(double idleTime, Action triggerAction, Action restartAction = null)
    {
        IdleTime = idleTime;
        TriggerAction = triggerAction;
        RestartAction = restartAction;
    }

    protected override void Update()
    {
        base.Update();

        if (alreadyTriggered) return;

        timeLeft -= Time.Elapsed;

        if (timeLeft > 0) return;

        TriggerAction?.Invoke();
        alreadyTriggered = true;
    }

    public void Reset()
    {
        if (alreadyTriggered)
            RestartAction?.Invoke();

        alreadyTriggered = false;
        timeLeft = IdleTime;
    }
}
