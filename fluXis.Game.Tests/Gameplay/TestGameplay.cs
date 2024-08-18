using System.Collections.Generic;
using fluXis.Game.Database.Maps;
using fluXis.Game.Graphics.Background;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Map;
using fluXis.Game.Mods;
using fluXis.Game.Screens;
using fluXis.Game.Screens.Gameplay;
using osu.Framework.Allocation;
using osu.Framework.Graphics;

namespace fluXis.Game.Tests.Gameplay;

public partial class TestGameplay : FluXisTestScene
{
    protected virtual List<IMod> Mods => new();

    [BackgroundDependencyLoader]
    private void load(MapStore maps)
    {
        CreateClock();

        var backgrounds = new GlobalBackground();
        TestDependencies.CacheAs(backgrounds);

        var screenStack = new FluXisScreenStack();
        TestDependencies.CacheAs(screenStack);

        AddRange(new Drawable[]
        {
            GlobalClock,
            backgrounds,
            screenStack
        });

        var map = GetTestMap(maps);

        if (map is null)
        {
            Add(new FluXisSpriteText
            {
                Text = "Test map could not be found.",
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                FontSize = 30
            });
            return;
        }

        AddStep("Push Loader", () =>
        {
            if (screenStack.CurrentScreen is not null)
                return;

            screenStack.Push(new GameplayLoader(map, Mods, () => CreateGameplayScreen(map)));
        });
    }

    protected virtual GameplayScreen CreateGameplayScreen(RealmMap map) => new(map, new List<IMod>());
}
