using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Overlay.Toolbar;

public partial class ToolbarSeparator : CircularContainer
{
    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Y;
        Width = 4;
        Height = 0.6f;
        Masking = true;
        Anchor = Anchor.CentreLeft;
        Origin = Anchor.CentreLeft;
        // Shear = new Vector2(0.1f, 0);
        Margin = new MarginPadding { Horizontal = 5 };
        Child = new Box
        {
            RelativeSizeAxes = Axes.Both
        };
    }
}
