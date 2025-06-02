using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Screens.Edit.UI.TabSwitcher;

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
    }
}
