using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Game.Screens.Edit.TabSwitcher;

public partial class EditorTabSwitcher : FillFlowContainer
{
    [BackgroundDependencyLoader]
    private void load()
    {
        Anchor = Anchor.TopRight;
        Origin = Anchor.TopRight;
        AutoSizeAxes = Axes.X;
        Direction = FillDirection.Horizontal;
        Height = 45;
        Padding = new MarginPadding(5);
    }
}
