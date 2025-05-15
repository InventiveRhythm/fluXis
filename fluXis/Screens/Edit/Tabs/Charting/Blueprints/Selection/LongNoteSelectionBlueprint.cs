using System;
using fluXis.Map.Structures;
using fluXis.Screens.Edit.Tabs.Charting.Playfield.Objects;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Screens.Edit.Tabs.Charting.Blueprints.Selection;

public partial class LongNoteSelectionBlueprint : NoteSelectionBlueprint
{
    public override Vector2 ScreenSpaceSelectionPoint => head.ScreenSpaceDrawQuad.Centre;

    private DraggableSelectionPiece head;
    private DraggableSelectionPiece end;

    private EditorLongNote drawable => Drawable as EditorLongNote;

    public override bool Visible => base.Visible || Math.Abs(EditorClock.CurrentTime - Object.EndTime) <= 4000;

    public LongNoteSelectionBlueprint(HitObject info)
        : base(info)
    {
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        Anchor = Origin = Anchor.BottomLeft;
        InternalChildren = new Drawable[]
        {
            head = new DraggableSelectionPiece
            {
                DragAction = dragStart,
                Anchor = Anchor.BottomLeft,
                Origin = Anchor.BottomLeft
            },
            end = new DraggableSelectionPiece
            {
                DragAction = dragEnd,
                Origin = Anchor.TopLeft
            },
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                Masking = true,
                BorderThickness = 1,
                BorderColour = Colour4.Yellow,
                Child = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Alpha = 0,
                    AlwaysPresent = true
                }
            }
        };
    }

    public override void UpdatePosition(Drawable parent)
    {
        base.UpdatePosition(parent);

        var delta = PositionProvider.PositionAtTime(Object.EndTime) - PositionProvider.PositionAtTime(Object.Time);
        Height = -(delta - drawable.End.DrawHeight);
    }

    private void dragStart(Vector2 vec)
    {
        var newTime = PositionProvider.TimeAtScreenSpacePosition(vec);
        newTime = Snaps.SnapTime(newTime);
        var newLen = Object.EndTime - newTime;

        if (newLen <= 10)
            return;

        Object.Time = newTime;
        Object.HoldTime = newLen;
    }

    private void dragEnd(Vector2 vec)
    {
        var newTime = PositionProvider.TimeAtScreenSpacePosition(vec);
        newTime = Snaps.SnapTime(newTime);
        var newLen = newTime - Object.Time;

        if (newLen <= 10)
            return;

        Object.EndTime = newTime;
    }
}
