using System;
using fluXis.Map.Structures.Bases;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;

namespace fluXis.Screens.Edit.Tabs.Charting.Playfield;

public abstract partial class EditorDrawableObject : CompositeDrawable
{
    [Resolved]
    protected EditorPlayfield Playfield { get; private set; }

    [Resolved]
    protected EditorClock EditorClock { get; private set; }

    [Resolved]
    private EditorSettings settings { get; set; }

    public ITimedObject Data { get; }

    public virtual bool Visible => Math.Abs(EditorClock.CurrentTime - Data.Time) <= 2000 / settings.Zoom;

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

    protected override void Update()
    {
        base.Update();

        Width = EditorHitObjectContainer.NOTEWIDTH * settings.ObjectZoom;

        X = Playfield.HitObjectContainer.PositionFromLane(Data.Lane);
        Y = Playfield.HitObjectContainer.PositionAtTime(Data.Time);
    }

    protected override bool OnHover(HoverEvent e) => true;
}
