using System.Collections.Generic;
using fluXis.Database.Maps;
using fluXis.Mods;
using fluXis.Screens.Gameplay;
using fluXis.Screens.Gameplay.Capabilities;

namespace fluXis.Tests.Gameplay;

public partial class TestReplay : TestGameplay
{
    protected override List<IMod> Mods { get; } = [new AutoPlayMod()];

    protected override GameplayScreen CreateGameplayScreen(RealmMap map)
    {
        var playable = map.GetPlayable(GameModes)!;
        var auto = GameModes.Find(playable.Mode)!.CreateAutoGenerator(playable);
        var replay = auto.Generate();
        return new GameplayScreen(map, Mods).RegisterCapability(new ReplayCapability(replay));
    }
}
