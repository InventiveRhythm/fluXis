using System;
using fluXis.Input;
using fluXis.Scoring.Enums;
using fluXis.Utils.Extensions;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;

namespace fluXis.Mode.Keys.Objects.Drawable;

public partial class DrawableTick : DrawableKeysHitObject<Tick>, IKeyBindingHandler<FluXisGameplayKeybind>
{
    public override bool CanBeRemoved => Judged || wouldMiss;
    private bool wouldMiss => Time.Current - Object.Time > HitWindows.TimingFor(HitWindows.LowestHitable);

    private int actionIndex;

    private bool heldDown;
    private bool directHit = false;
    private double? holdStartTime;

    public DrawableTick(Tick o)
        : base(o)
    {
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChild = Skin.GetTickNote(PlayerLane, Manager.KeyCount, Object.Small).WithRelativeSize(Axes.X);
        actionIndex = Array.IndexOf(Keybinds.Keys, Action);
    }

    protected override void Update()
    {
        base.Update();

        heldDown = Keybinds.PressedActions.Contains(Action);
        holdStartTime = heldDown ? Keybinds.PressTimes[actionIndex] : null;

        if (heldDown) UpdateJudgement(true);
    }

    protected override void CheckJudgement(bool byUser, double offset)
    {
        // TODO: i don't think any of this works properly with the new system

        if (!byUser)
        {
            ApplyResult(HitWindows.JudgementFor(lagCompensation() ?? HitWindows.TimingFor(HitWindows.Lowest)));
            return;
        }

        if (offset >= 0 && !directHit)
            return;

        if (wouldMiss)
        {
            var off = lagCompensation();

            if (off != null)
            {
                ApplyResult(HitWindows.JudgementFor(off.Value));
                return;
            }
        }

        // ObjectManager.PlayHitSound(Data, false);
        ApplyResult(HitWindows.JudgementFor(lagCompensation() ?? offset));
        return;

        double? lagCompensation()
        {
            if (heldDown && holdStartTime != null)
            {
                var delta = holdStartTime.Value - Object.Time;
                return delta < 0 ? 0 : delta;
            }

            return null;
        }
    }

    public bool OnPressed(KeyBindingPressEvent<FluXisGameplayKeybind> e)
    {
        if (e.Action != Action)
            return false;

        var flWindow = HitWindows.TimingFor(Judgement.Flawless);

        if (Math.Abs(TimeDelta) < flWindow)
        {
            directHit = true;
            UpdateJudgement(true);
            return true;
        }

        return false;
    }

    public void OnReleased(KeyBindingReleaseEvent<FluXisGameplayKeybind> e)
    {
        if (e.Action != Action)
            return;

        UpdateJudgement(true);
    }
}
