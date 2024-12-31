using System.Collections.Generic;
using fluXis.Database.Maps;
using fluXis.Mods;
using fluXis.Replays;
using fluXis.Screens.Gameplay;
using fluXis.Screens.Gameplay.Replays;

namespace fluXis.Tests.Gameplay;

public partial class TestReplay : TestGameplay
{
    protected override List<IMod> Mods { get; } = new() { new AutoPlayMod() };

    protected override GameplayScreen CreateGameplayScreen(RealmMap map)
    {
        var info = map.GetMapInfo();
        var auto = new AutoGenerator(info, map.KeyCount);
        var replay = auto.Generate();

        return new ReplayGameplayScreen(map, Mods, replay);
    }
}
