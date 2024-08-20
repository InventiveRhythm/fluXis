using fluXis.Game.Map.Structures;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Blueprints.Selection;

public partial class LongNoteSelectionBlueprint : NoteSelectionBlueprint
{
    public override Vector2 ScreenSpaceSelectionPoint => head.ScreenSpaceDrawQuad.Centre;

    private DraggableSelectionPiece head;
    private DraggableSelectionPiece end;

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
                Origin = Anchor.CentreLeft
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

    protected override void Update()
    {
        base.Update();

        var delta = HitObjectContainer.PositionAtTime(Object.EndTime) - HitObjectContainer.PositionAtTime(Object.Time);
        Height = -(delta - Drawable.LongNoteEnd.DrawHeight / 2);
    }

    private void dragStart(Vector2 vec)
    {
        var newTime = HitObjectContainer.TimeAtScreenSpacePosition(vec);
        newTime = Snaps.SnapTime(newTime);
        var newLen = Object.EndTime - newTime;

        if (newLen <= 10)
            return;

        Object.Time = newTime;
        Object.HoldTime = newLen;
    }

    private void dragEnd(Vector2 vec)
    {
        var newTime = HitObjectContainer.TimeAtScreenSpacePosition(vec);
        newTime = Snaps.SnapTime(newTime);
        var newLen = newTime - Object.Time;

        if (newLen <= 10)
            return;

        Object.EndTime = newTime;
    }
}
