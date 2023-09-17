using fluXis.Game.Graphics.UserInterface.Color;
using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osuTK.Graphics;

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
        EdgeEffect = new EdgeEffectParameters
        {
            Type = EdgeEffectType.Shadow,
            Colour = Color4.Black.Opacity(.25f),
            Radius = 5
        };

        InternalChild = new Box
        {
            RelativeSizeAxes = Axes.Both,
            Colour = FluXisColors.Background1
        };
    }
}
