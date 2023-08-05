using fluXis.Game.Screens.Edit.Tabs.Charting;
using osu.Framework.Allocation;

namespace fluXis.Game.Screens.Edit.Tabs;

public partial class ChartingTab : EditorTab
{
    public ChartingTab(Editor screen)
        : base(screen)
    {
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        Child = new ChartingContainer();
    }
}
