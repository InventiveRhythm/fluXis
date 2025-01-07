using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Graphics.UserInterface.Tabs;

public abstract partial class TabContainer : Container
{
    public abstract IconUsage Icon { get; }
    public abstract string Title { get; }

    protected TabContainer()
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
    }
}
