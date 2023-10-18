using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Game.Overlay.Settings.UI;

public partial class SettingsDivider : CircularContainer
{
    [BackgroundDependencyLoader]
    private void load()
    {
        Masking = true;
        CornerRadius = 5;
        RelativeSizeAxes = Axes.X;
        Height = 3;
        Margin = new MarginPadding { Vertical = 10 };

        Child = new Box
        {
            RelativeSizeAxes = Axes.Both,
            Alpha = 0.5f
        };
    }
}
