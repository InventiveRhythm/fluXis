using fluXis.Game.Map.Structures;
using osu.Framework.Allocation;
using osu.Framework.Graphics;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Blueprints.Selection;

public partial class SingleNoteSelectionBlueprint : NoteSelectionBlueprint
{
    public SingleNoteSelectionBlueprint(HitObject info)
        : base(info)
    {
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChild = new BlueprintNotePiece
        {
            RelativeSizeAxes = Axes.X,
            Width = 0.5f,
            Anchor = Anchor.Centre
        };
    }

    protected override void Update()
    {
        base.Update();
        Height = Drawable.HitObjectPiece.DrawHeight;
    }
}
