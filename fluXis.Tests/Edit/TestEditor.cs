using fluXis.Graphics.Background;
using fluXis.Graphics.UserInterface.Panel;
using fluXis.Map;
using fluXis.Overlay.Notifications;
using fluXis.Screens;
using fluXis.Screens.Edit;
using osu.Framework.Allocation;

namespace fluXis.Tests.Edit;

public partial class TestEditor : FluXisTestScene
{
    [Resolved]
    private MapStore maps { get; set; }

    protected override bool UseTestAPI => true;

    [BackgroundDependencyLoader]
    private void load(NotificationManager notifications)
    {
        CreateClock();

        var backgrounds = new GlobalBackground();
        TestDependencies.CacheAs(backgrounds);

        var screenStack = new FluXisScreenStack();
        TestDependencies.CacheAs(screenStack);

        var panels = new PanelContainer();
        TestDependencies.CacheAs(panels);

        var floating = new FloatingNotificationContainer();
        notifications.Floating = floating;
        TestDependencies.CacheAs(floating);

        Add(GlobalClock);
        Add(backgrounds);
        Add(screenStack);
        Add(panels);
        Add(floating);

        AddStep("Push existing map", () =>
        {
            var map = GetTestMap(maps);
            var loader = map is not null ? new EditorLoader(map, map.GetMapInfo()) : new EditorLoader();
            loader.StartTabIndex = EditorTabType.Setup;
            screenStack.Push(loader);
        });

        AddStep("Push new map", () => screenStack.Push(new EditorLoader()));
    }
}
