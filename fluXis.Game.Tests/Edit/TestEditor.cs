using System.Linq;
using fluXis.Game.Graphics.Background;
using fluXis.Game.Graphics.UserInterface.Panel;
using fluXis.Game.Map;
using fluXis.Game.Screens;
using fluXis.Game.Screens.Edit;
using osu.Framework.Allocation;

namespace fluXis.Game.Tests.Edit;

public partial class TestEditor : FluXisTestScene
{
    [Resolved]
    private MapStore maps { get; set; }

    [BackgroundDependencyLoader]
    private void load()
    {
        CreateClock();

        var backgrounds = new GlobalBackground();
        TestDependencies.CacheAs(backgrounds);

        var screenStack = new FluXisScreenStack();
        TestDependencies.CacheAs(screenStack);

        var panels = new PanelContainer();
        TestDependencies.CacheAs(panels);

        Add(GlobalClock);
        Add(backgrounds);
        Add(screenStack);
        Add(panels);

        AddStep("Push existing map", () =>
        {
            var map = maps.GetFromGuid("f2c5760f-0ebb-4498-9008-f6e52292f5a6")?
                .Maps.FirstOrDefault();

            var editor = map is not null ? new EditorLoader(map, map.GetMapInfo()) : new EditorLoader();
            screenStack.Push(editor);
        });

        AddStep("Push new map", () =>
        {
            var editor = new EditorLoader();
            screenStack.Push(editor);
        });
    }
}
