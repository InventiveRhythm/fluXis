using System;
using fluXis.Input;
using fluXis.Map.Structures;
using fluXis.Modes.Keys.Gameplay;
using fluXis.Scoring;
using fluXis.Screens.Gameplay.Ruleset;
using fluXis.Skinning;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;

namespace fluXis.Modes.Keys.HitObjects;

public abstract partial class DrawableHitObject : CompositeDrawable, IKeyBindingHandler<FluXisGameplayKeybind>
{
    [Resolved]
    protected RulesetContainer Ruleset { get; private set; }

    [Resolved]
    protected HitObjectManager ObjectManager { get; private set; }

    [Resolved]
    protected HitObjectColumn Column { get; private set; }

    [Resolved]
    protected KeysKeybindContainer Keybinds { get; private set; }

    [Resolved]
    public ISkin Skin { get; private set; }

    public HitObject Data { get; }
    protected double ScrollVelocityTime { get; private set; }
    protected double ScrollVelocityEndTime { get; private set; }

    protected double TimeDelta => Data.Time - Time.Current;

    protected int VisualLane
    {
        get
        {
            var lane = Data.Lane;

            while (lane > ObjectManager.KeyCount)
                lane -= ObjectManager.KeyCount;

            return lane;
        }
    }

    public FluXisGameplayKeybind Keybind { get; set; }

    public virtual bool CanBeRemoved => false;
    public virtual HitWindows HitWindows => Ruleset.HitWindows;

    public bool Judged { get; private set; }
    public Action<DrawableHitObject, double> OnHit { get; set; }

    protected DrawableHitObject(HitObject data)
    {
        Data = data;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        AutoSizeAxes = Axes.Y;
        Origin = Anchor.BottomLeft;
        Masking = true;

        var group = Data.ScrollGroup ?? Column.DefaultScrollGroup;
        ScrollVelocityTime = group.PositionFromTime(Data.Time);
        ScrollVelocityEndTime = group.PositionFromTime(Data.EndTime);
    }

    protected override void Update()
    {
        base.Update();

        X = ObjectManager.PositionAtLane(Data.Lane);
        Y = Column.PositionAtTime(ScrollVelocityTime, Data.ScrollGroup, Data.StartEasing);
        Width = ObjectManager.WidthOfLane(Data.Lane);
    }

    protected bool UpdateJudgement(bool byUser)
    {
        if (Judged)
            return false;

        CheckJudgement(byUser, TimeDelta);
        return Judged;
    }

    protected virtual void CheckJudgement(bool byUser, double offset) { }

    protected void ApplyResult(double diff)
    {
        if (Judged)
            throw new InvalidOperationException("Can not apply judgement to already judged hitobject.");

        Judged = true;

        OnHit?.Invoke(this, diff);
    }

    public void OnKill()
    {
        UpdateJudgement(false);
    }

    protected abstract bool OnPressed(FluXisGameplayKeybind bind);
    protected virtual void OnReleased(FluXisGameplayKeybind bind) { }

    bool IKeyBindingHandler<FluXisGameplayKeybind>.OnPressed(KeyBindingPressEvent<FluXisGameplayKeybind> e) => OnPressed(e.Action);
    void IKeyBindingHandler<FluXisGameplayKeybind>.OnReleased(KeyBindingReleaseEvent<FluXisGameplayKeybind> e) => OnReleased(e.Action);
}
