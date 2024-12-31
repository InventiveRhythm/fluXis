using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Screens.Select.List.Drawables;

public partial class RoundedOutline : Container
{
    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;
        Masking = true;
        CornerRadius = 10;
        BorderThickness = 3;

        Child = new Box
        {
            RelativeSizeAxes = Axes.Both,
            Colour = Colour4.Transparent
        };
    }
}
