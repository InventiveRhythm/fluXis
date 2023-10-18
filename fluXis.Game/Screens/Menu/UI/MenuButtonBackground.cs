using fluXis.Game.Graphics;
using fluXis.Game.Graphics.UserInterface.Color;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Game.Screens.Menu.UI;

public partial class MenuButtonBackground : Container
{
    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Width = 1.2f;
        Height = 50;
        Anchor = Anchor.TopRight;
        Origin = Anchor.CentreRight;
        X = 10;
        Masking = true;
        CornerRadius = 10;
        EdgeEffect = FluXisStyles.ShadowSmall;

        InternalChild = new Box
        {
            RelativeSizeAxes = Axes.Both,
            Colour = FluXisColors.Background1
        };
    }
}
