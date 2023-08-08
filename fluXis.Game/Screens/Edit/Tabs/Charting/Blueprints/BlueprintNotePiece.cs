using fluXis.Game.Graphics;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Blueprints;

public partial class BlueprintNotePiece : Container
{
    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Width = 0.5f;
        Height = 10;
        CornerRadius = 5;
        Masking = true;
        Anchor = Anchor.BottomCentre;
        Origin = Anchor.Centre;
        Child = new Box
        {
            RelativeSizeAxes = Axes.Both,
            Colour = FluXisColors.Selection
        };
    }
}
