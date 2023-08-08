using fluXis.Game.Map;
using fluXis.Game.Screens.Edit.Tabs.Charting.Blueprints;
using osu.Framework.Allocation;
using osu.Framework.Graphics;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Selection;

public partial class SingleNoteSelectionBlueprint : SelectionBlueprint
{
    public SingleNoteSelectionBlueprint(HitObjectInfo info)
        : base(info)
    {
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChild = new BlueprintNotePiece().With(b => b.Y = -Drawable.HitObjectPiece.DrawHeight / 2);
    }

    protected override void Update()
    {
        base.Update();
        Height = Drawable.HitObjectPiece.DrawHeight;
    }
}
