using System.Collections.Generic;
using fluXis.Database.Maps;
using fluXis.Mods;
using fluXis.Replays;
using fluXis.Screens.Gameplay.Replays;
using fluXis.Screens.Gameplay.Ruleset;

namespace fluXis.Screens.Gameplay.Spectator;

public partial class SpectatorGameplay : ReplayGameplayScreen
{
    public SpectatorGameplay(RealmMap realmMap, List<IMod> mods, Replay replay)
        : base(realmMap, mods, replay)
    {
    }

    protected override RulesetContainer CreateRuleset()
    {
        var rs = base.CreateRuleset();
        ((ReplayRulesetContainer)rs).RequireSyncFrames = true;
        return rs;
    }
}
