using System;
using fluXis.Map.Structures.Bases;
using fluXis.Screens.Edit.Blueprints.Selection;
using fluXis.Screens.Edit.Tabs.Charting.Playfield;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Primitives;
using osuTK;

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

    public ChartingSelectionBlueprint(ITimedObject obj)
        : base(obj)
    {
        Width = EditorHitObjectContainer.NOTEWIDTH;

        InternalChildren =
        [
            new BlueprintNotePiece
            {
                RelativeSizeAxes = Axes.X,
                Width = 0.5f,
                Anchor = Anchor.Centre
            }
        ];
    }

    public override void UpdatePosition(Drawable parent)
    {
        base.UpdatePosition(parent);

        if (parent != null)
            Position = parent.ToLocalSpace(PositionProvider.ScreenSpacePositionAtTime(Object.Time, Object.Lane));

        /*if (Object is IHasDuration { Duration: > 0 } d)
        {
            var delta = PositionProvider.PositionAtTime(d.GetEndTime()) - PositionProvider.PositionAtTime(Object.Time);
            Height = -(delta - (Drawable as EditorLongNote)?.End.DrawHeight ?? 0);
        }
        else*/
        Height = Drawable.DrawHeight;
    }

    /*
       protected override bool OnMouseDown(MouseDownEvent e)
       {
           if (Object.Type != HitObjectType.Tick || e.Button != MouseButton.Middle)
               return false;

           Object.HoldTime = Object.HoldTime > 0 ? 0 : 1;
           return true;
       }
     */
}
