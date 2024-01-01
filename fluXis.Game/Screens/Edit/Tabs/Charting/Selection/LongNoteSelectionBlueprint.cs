using fluXis.Game.Map.Structures;
using fluXis.Game.Screens.Edit.Tabs.Charting.Blueprints;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Selection;

public partial class LongNoteSelectionBlueprint : NoteSelectionBlueprint
{
    public override Vector2 ScreenSpaceSelectionPoint => head.ScreenSpaceDrawQuad.Centre;

    private Container head;
    private Container end;

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
            head = new BlueprintNotePiece
            {
                Y = -Drawable.HitObjectPiece.DrawHeight / 2,
                RelativeSizeAxes = Axes.X,
                Width = 0.5f
            },
            end = new BlueprintNotePiece
            {
                RelativeSizeAxes = Axes.X,
                Width = 0.5f
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
        end.Y = delta;
        Height = -(delta - Drawable.LongNoteEnd.DrawHeight / 2);
    }
}
