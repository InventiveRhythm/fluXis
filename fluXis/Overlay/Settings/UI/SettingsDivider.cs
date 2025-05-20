using fluXis.Graphics.UserInterface.Color;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Overlay.Settings.UI;

public partial class SettingsDivider : CircularContainer
{
    [BackgroundDependencyLoader]
    private void load()
    {
        Masking = true;
        CornerRadius = 2;
        RelativeSizeAxes = Axes.X;
        Height = 4;
        Margin = new MarginPadding { Vertical = 12 };

        Child = new Box
        {
            RelativeSizeAxes = Axes.Both,
            Colour = FluXisColors.Background3
        };
    }
}
