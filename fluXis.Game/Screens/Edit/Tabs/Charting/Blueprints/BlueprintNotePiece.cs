using fluXis.Game.Graphics.UserInterface.Color;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Game.Screens.Edit.Tabs.Charting.Blueprints;

public partial class BlueprintNotePiece : Container
{
    public BlueprintNotePiece()
    {
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
