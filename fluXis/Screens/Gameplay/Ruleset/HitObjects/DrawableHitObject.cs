using System;
using System.Collections.Generic;
using fluXis.Input;
using fluXis.Map.Structures;
using fluXis.Scoring;
using fluXis.Screens.Gameplay.Input;
using fluXis.Screens.Gameplay.Ruleset.HitObjects.Long;
using fluXis.Screens.Gameplay.Ruleset.Playfields;
using fluXis.Skinning;
using fluXis.Skinning.Bases;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Screens.Gameplay.Ruleset.HitObjects;

public partial class DrawableHitObject : CompositeDrawable
{
    [Resolved]
    protected RulesetContainer Ruleset { get; private set; }

    [Resolved]
    protected HitObjectManager ObjectManager { get; private set; }

    [Resolved]
    protected HitObjectColumn Column { get; private set; }

    [Resolved]
    protected GameplayInput Input { get; private set; }

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

    protected override void LoadComplete()
    {
        base.LoadComplete();

        Input.OnPress += OnPressed;
        Input.OnRelease += OnReleased;
    }

    public IEnumerable<ColorableSkinDrawable> GetColorableDrawables()
    {
        foreach (var child in InternalChildren)
        {
            switch (child)
            {
                case ColorableSkinDrawable drawable:
                    yield return drawable;
                    break;
                    
                default:
                    if (child.Parent is DrawableLongNote drawableLongNote)
                    {
                        foreach (var longNoteDrawable in getLongNoteColorableDrawables(drawableLongNote))
                        {
                            yield return longNoteDrawable;
                        }
                    }
                    break;
            }
        }
    }

    private IEnumerable<ColorableSkinDrawable> getLongNoteColorableDrawables(DrawableLongNote longNote)
    {
        foreach (var lnChild in longNote.InternalChildren)
        {
            if (lnChild is DrawableLongNotePart notePart)
            {
                foreach (var notePartChild in notePart.GetInternalChildren())
                {
                    if (notePartChild is ColorableSkinDrawable drawable)
                        yield return drawable;
                } 
            }
        }
    }

    public void SetColor(Colour4 colour)
    {
        foreach (var drawable in GetColorableDrawables())
        {
            drawable.SetColor(colour);
        }
    }

    protected override void Update()
    {
        base.Update();

        X = ObjectManager.PositionAtLane(Data.Lane);
        Y = Column.PositionAtTime(ScrollVelocityTime, Data.ScrollGroup, Data.StartEasing);
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

        CheckJudgement(byUser, TimeDelta);
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
