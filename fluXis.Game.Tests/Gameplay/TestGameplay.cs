using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Graphics.Background;
using fluXis.Game.Map;
using fluXis.Game.Mods;
using fluXis.Game.Screens;
using fluXis.Game.Screens.Gameplay;
using osu.Framework.Allocation;
using osu.Framework.Graphics;

namespace fluXis.Game.Tests.Gameplay;

public partial class TestGameplay : FluXisTestScene
{
    [BackgroundDependencyLoader]
    private void load(MapStore maps)
    {
        CreateClock();

        var backgrounds = new GlobalBackground();
        TestDependencies.CacheAs(backgrounds);

        var screenStack = new FluXisScreenStack();
        TestDependencies.CacheAs(screenStack);

        const string set_id = "9896365c-5541-4612-9f39-5a44aa1012ed";
        const string map_id = "3b55b380-e533-4eea-bf16-4b98d9776583";

        var map = maps.GetFromGuid(set_id)?.Maps.FirstOrDefault(m => m.ID == Guid.Parse(map_id));
        if (map is null) return;

        AddRange(new Drawable[]
        {
            GlobalClock,
            backgrounds,
            screenStack
        });

        AddStep("Push Loader", () =>
        {
            if (screenStack.CurrentScreen is not null) return;

            screenStack.Push(new GameplayLoader(map, new List<IMod>(), () => new GameplayScreen(map, new List<IMod>())));
        });
    }
}
