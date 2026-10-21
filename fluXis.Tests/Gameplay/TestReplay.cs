using System;
using System.Collections.Generic;
using fluXis.Database.Maps;
using fluXis.Modes;
using fluXis.Mods;
using fluXis.Screens.Gameplay;
using fluXis.Screens.Gameplay.Capabilities;

namespace fluXis.Tests.Gameplay;

public partial class TestReplay : TestGameplay
{
    protected override List<IMod> Mods { get; } = new() { new AutoPlayMod() };

    protected override GameplayScreen CreateGameplayScreen(RealmMap map)
    {
        var playable = map.GetPlayable(Modes);
        var mode = Modes.Find(playable?.Mode ?? throw new NullReferenceException("could not get mode for playable")) ?? throw GameModeManager.FailedToLoadException();
        var auto = mode.CreateAutoGenerator(playable);
        var replay = auto.Generate();
        return new GameplayScreen(map, Mods).RegisterCapability(new ReplayCapability(replay));
    }
}
