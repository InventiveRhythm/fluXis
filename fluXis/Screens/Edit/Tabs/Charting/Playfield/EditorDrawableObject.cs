using System;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Map.Structures.Bases;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;

namespace fluXis.Screens.Edit.Tabs.Charting.Playfield;

public abstract partial class EditorDrawableObject : CompositeDrawable
{
    [Resolved]
    protected EditorMap Map { get; private set; }

    [Resolved]
    protected EditorPlayfield Playfield { get; private set; }

    [Resolved]
    protected EditorClock EditorClock { get; private set; }

    [Resolved]
    private EditorSettings settings { get; set; }

    public ITimedObject Data { get; }
    public event Action DataUpdate;

    public float Zoom => settings.ObjectZoom;

    public virtual bool Visible => Math.Abs(EditorClock.CurrentTime - Data.Time) <= 2000 / settings.Zoom;
    public virtual Colour4 TextColor => Theme.TextDark;

    [CanBeNull]
    public FluXisSpriteText GroupText;

    protected EditorDrawableObject(ITimedObject hit)
    {
        Data = hit;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        AutoSizeAxes = Axes.Y;
        Origin = Anchor.BottomLeft;
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        Map.AnyChange += onChange;
        UpdateOverlay();
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);
        Map.AnyChange -= onChange;
    }

    private void onChange([CanBeNull] ITimedObject obj)
    {
        if (obj != Data)
            return;

        Scheduler.AddOnce(UpdateOverlay);
    }

    protected virtual void UpdateOverlay()
    {
        if (GroupText != null)
            GroupText.Text = Data.Group;

        DataUpdate?.Invoke();
    }

    protected override void Update()
    {
        base.Update();

        Width = EditorHitObjectContainer.NOTEWIDTH * settings.ObjectZoom;

        X = Playfield.HitObjectContainer.PositionFromLane(Data.Lane);
        Y = Playfield.HitObjectContainer.PositionAtTime(Data.Time);
    }

    protected override bool OnHover(HoverEvent e) => true;
}
