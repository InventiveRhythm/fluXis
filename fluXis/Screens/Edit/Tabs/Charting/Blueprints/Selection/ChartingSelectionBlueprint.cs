using System;
using fluXis.Audio;
using fluXis.Map.Structures;
using fluXis.Map.Structures.Bases;
using fluXis.Screens.Edit.Blueprints.Selection;
using fluXis.Screens.Edit.Tabs.Charting.Playfield;
using osu.Framework.Allocation;
using osu.Framework.Audio.Sample;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Input;

namespace fluXis.Screens.Edit.Tabs.Charting.Blueprints.Selection;

public partial class ChartingSelectionBlueprint : SelectionBlueprint<ITimedObject>
{
    [Resolved]
    protected EditorSnapProvider Snaps { get; private set; }

    [Resolved]
    protected EditorMap Map { get; private set; }

    [Resolved]
    protected ChartingContainer ChartingContainer { get; private set; }

    [Resolved]
    protected EditorClock EditorClock { get; private set; }

    [Resolved]
    protected EditorSettings EditorSettings { get; private set; }

    public override RectangleF ScreenSpaceSelectionRect
    {
        get
        {
            var pos = PositionProvider.ScreenSpacePositionAtTime(Object.Time, Object.Lane);
            var size = Drawable.ScreenSpaceDrawQuad.Size;
            pos -= new Vector2(0, size.Y);
            return new RectangleF(pos, size);
        }
    }

    protected ITimePositionProvider PositionProvider => ChartingContainer.Playfield;

    public new EditorDrawableObject Drawable => base.Drawable as EditorDrawableObject;

    public override double FirstComparer => Object.Time;
    public override double SecondComparer => Object is IHasDuration { Duration: > 0 } d ? d.GetEndTime() : Object.Lane;

    public override bool Visible
    {
        get
        {
            var visible = Math.Abs(EditorClock.CurrentTime - Object.Time) <= 4000;

            if (Object is IHasDuration { Duration: > 0 } d)
                visible = visible || Math.Abs(EditorClock.CurrentTime - d.GetEndTime()) <= 4000;

            return visible;
        }
    }

    private readonly BlueprintNotePiece piece;
    private readonly DraggableSelectionPiece head;
    private readonly DraggableSelectionPiece end;

    private DebouncedSample sample;

    public ChartingSelectionBlueprint(ITimedObject obj)
        : base(obj)
    {
        InternalChildren =
        [
            piece = new BlueprintNotePiece
            {
                RelativeSizeAxes = Axes.X,
                Width = 0.5f,
                Anchor = Anchor.Centre
            },
            head = new DraggableSelectionPiece
            {
                DragAction = dragStart,
                Anchor = Anchor.BottomLeft,
                Origin = Anchor.BottomLeft,
                Alpha = 0
            },
            end = new DraggableSelectionPiece
            {
                DragAction = dragEnd,
                Origin = Anchor.TopLeft,
                Alpha = 0
            },
        ];
    }

    [BackgroundDependencyLoader]
    private void load(ISampleStore samples)
    {
        sample = new DebouncedSample(samples.Get("UI/slider-tick"));
        AddInternal(sample);
    }

    public override void UpdatePosition(Drawable parent)
    {
        base.UpdatePosition(parent);

        if (IsSelected)
        {
            if (Object is IHasDuration { Duration: > 0 })
            {
                piece.Alpha = 0;
                end.Alpha = head.Alpha = 1f;
            }
            else
            {
                piece.Alpha = 1;
                end.Alpha = head.Alpha = 0;
            }
        }

        Width = EditorHitObjectContainer.NOTEWIDTH * EditorSettings.ObjectZoom;

        if (parent != null)
            Position = parent.ToLocalSpace(PositionProvider.ScreenSpacePositionAtTime(Object.Time, Object.Lane));

        Height = Drawable.DrawHeight;
    }

    protected override bool OnMouseDown(MouseDownEvent e)
    {
        if (e.ShiftPressed && e.Button == MouseButton.Left)
        {
            ChartingContainer.BlueprintContainer.SelectionHandler.DeselectAll();
            ChartingContainer.Sidebar.ShowPoint(Object);
            Select();
            return true;
        }

        if (Object is not HitObject { Type: HitObjectType.Tick } h || e.Button != MouseButton.Middle)
            return false;

        h.HoldTime = h.HoldTime > 0 ? 0 : 1;
        return true;
    }

    private void dragStart(Vector2 vec)
    {
        if (Object is not IHasDuration d)
            return;

        var newTime = PositionProvider.TimeAtScreenSpacePosition(vec);
        newTime = Snaps.SnapTime(newTime);
        var newLen = d.GetEndTime() - newTime;

        if (newLen <= 10)
            return;

        if (Math.Abs(d.Time - newTime) > 0.1f)
            sample?.Play();

        d.Time = newTime;
        d.Duration = newLen;
    }

    private void dragEnd(Vector2 vec)
    {
        if (Object is not IHasDuration d)
            return;

        var newTime = PositionProvider.TimeAtScreenSpacePosition(vec);
        newTime = Snaps.SnapTime(newTime);
        var newLen = newTime - Object.Time;

        if (newLen <= 10)
            return;

        if (Math.Abs(d.GetEndTime() - newTime) > 0.1f)
            sample?.Play();

        d.Duration = newTime - d.Time;
    }
}
