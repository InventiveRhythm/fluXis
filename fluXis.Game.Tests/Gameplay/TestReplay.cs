using System.Collections.Generic;
using fluXis.Game.Database.Maps;
using fluXis.Game.Mods;
using fluXis.Game.Replays;
using fluXis.Game.Screens.Gameplay;
using fluXis.Game.Screens.Gameplay.Replays;

namespace fluXis.Game.Tests.Gameplay;

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
