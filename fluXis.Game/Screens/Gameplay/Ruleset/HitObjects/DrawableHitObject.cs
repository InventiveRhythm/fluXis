using System;
using fluXis.Game.Input;
using fluXis.Game.Map.Structures;
using fluXis.Game.Scoring;
using fluXis.Game.Screens.Gameplay.Input;
using fluXis.Game.Skinning;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Game.Screens.Gameplay.Ruleset.HitObjects;

public partial class DrawableHitObject : CompositeDrawable
{
    [Resolved]
    protected GameplayScreen Screen { get; private set; }

    [Resolved]
    protected HitObjectManager ObjectManager { get; private set; }

    [Resolved]
    protected HitObjectColumn Column { get; private set; }

    [Resolved]
    protected GameplayInput Input { get; private set; }

    [Resolved]
    protected SkinManager SkinManager { get; private set; }

    public HitObject Data { get; }
    protected double ScrollVelocityTime { get; private set; }
    protected double ScrollVelocityEndTime { get; private set; }

    public FluXisGameplayKeybind Keybind { get; set; }

    public virtual bool CanBeRemoved => false;
    public virtual HitWindows HitWindows => Screen.HitWindows;

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

        ScrollVelocityTime = Column.ScrollVelocityPositionFromTime(Data.Time);
        ScrollVelocityEndTime = Column.ScrollVelocityPositionFromTime(Data.EndTime);
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        Input.OnPress += OnPressed;
        Input.OnRelease += OnReleased;
    }

    protected override void Update()
    {
        base.Update();

        X = ObjectManager.PositionAtLane(Data.Lane);
        Y = Column.PositionAtTime(ScrollVelocityTime, Data.StartEasing);
        Width = ObjectManager.WidthOfLane(Data.Lane);
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);

        Input.OnPress -= OnPressed;
        Input.OnRelease -= OnReleased;
    }

    protected void UpdateJudgement(bool byUser)
    {
        if (Judged)
            return;

        var offset = Data.Time - Time.Current;
        CheckJudgement(byUser, offset);
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

    public virtual void OnPressed(FluXisGameplayKeybind key) { }
    public virtual void OnReleased(FluXisGameplayKeybind key) { }
}
