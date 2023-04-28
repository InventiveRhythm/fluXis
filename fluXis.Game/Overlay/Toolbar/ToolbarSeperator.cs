using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Game.Overlay.Toolbar;

public partial class ToolbarSeperator : CircularContainer
{
    [BackgroundDependencyLoader]
    private void load()
    {
        Height = 30;
        Width = 4;
        Masking = true;
        Anchor = Anchor.CentreLeft;
        Origin = Anchor.CentreLeft;
        Shear = new Vector2(0.1f, 0);
        Margin = new MarginPadding { Horizontal = 5 };
        Child = new Box
        {
            RelativeSizeAxes = Axes.Both
        };
    }
}
