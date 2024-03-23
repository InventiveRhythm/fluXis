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

        var map = maps.GetFromGuid("127b9249-d17a-4ee7-9629-3c2691478d8b")?
            .Maps.FirstOrDefault();

        var editor = map is not null ? new EditorLoader(map, map.GetMapInfo()) : new EditorLoader();
        screenStack.Push(editor);
    }
}
