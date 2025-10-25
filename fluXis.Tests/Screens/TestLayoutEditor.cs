using fluXis.Graphics.Background;
using fluXis.Graphics.UserInterface.Panel;
using fluXis.Map;
using fluXis.Screens.Gameplay.HUD;
using fluXis.Screens.Layout;
using osu.Framework.Allocation;
using osu.Framework.Screens;

namespace fluXis.Tests.Screens;

public partial class TestLayoutEditor : FluXisTestScene
{
    [BackgroundDependencyLoader]
    private void load(MapStore maps, LayoutManager manager)
    {
        CreateClock();
        maps.CurrentMap = GetTestMap(maps);

        if (manager.IsDefault)
            manager.CreateNewLayout(false);

        var stack = new ScreenStack();
        Add(stack);

        var panels = new PanelContainer();
        TestDependencies.CacheAs(panels);
        TestDependencies.CacheAs(new GlobalBackground());
        Add(panels);

        AddStep("Push Screen", () => stack.Push(new LayoutEditor(manager.Layout.Value)));
    }
}
