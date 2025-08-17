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

        var background = new GlobalBackground();
        TestDependencies.Cache(background);
        Add(background);

        var stack = new ScreenStack();
        Add(stack);

        var panels = new PanelContainer();
        TestDependencies.CacheAs(panels);
        Add(panels);

        AddStep("Push Screen", () => stack.Push(new LayoutEditor(manager.Layout.Value)));
    }
}
