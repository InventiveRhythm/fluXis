using fluXis.Game.Map.Structures.Events;
using fluXis.Game.Screens.Edit.Tabs.Charting.Effect;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Blueprints.Selection.Effect;

public partial class FlashSelectionBlueprint : ChartingSelectionBlueprint
{
    public FlashEvent Flash => (FlashEvent)Object;

    public new EditorFlashEvent Drawable => (EditorFlashEvent)base.Drawable;

    public FlashSelectionBlueprint(FlashEvent flash)
        : base(flash)
    {
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        AutoSizeAxes = Axes.X;
        InternalChild = new Box
        {
            Width = 10,
            RelativeSizeAxes = Axes.Y,
            Colour = Colour4.Transparent
        };
    }

    protected override void Update()
    {
        base.Update();

        if (Parent != null)
            Position = Parent.ToLocalSpace(Drawable.ScreenSpaceDrawQuad.BottomLeft);

        Height = Drawable.DrawHeight;
    }
}
