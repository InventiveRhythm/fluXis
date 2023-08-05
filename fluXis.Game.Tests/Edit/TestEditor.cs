using System;
using System.Linq;
using fluXis.Game.Graphics.Background;
using fluXis.Game.Map;
using fluXis.Game.Screens.Edit;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Screens;

namespace fluXis.Game.Tests.Edit;

public partial class TestEditor : FluXisTestScene
{
    [Resolved]
    private BackgroundStack backgroundStack { get; set; }

    [Resolved]
    private MapStore maps { get; set; }

    private ScreenStack screenStack { get; } = new() { RelativeSizeAxes = Axes.Both };
    private Editor editor { get; set; }

    [BackgroundDependencyLoader]
    private void load()
    {
        Add(backgroundStack);
        Add(screenStack);

        var map = maps.MapSets.FirstOrDefault(s => s.ID == Guid.Parse("3238381d-f8f1-4d68-a88f-427ad9821eb1"))?
            .Maps.FirstOrDefault(m => m.ID == Guid.Parse("979702d3-e039-4b36-8f51-433ca28df679"));

        editor = map is not null ? new Editor(map, map.GetMapInfo()) : new Editor();
        screenStack.Push(editor);
    }
}
