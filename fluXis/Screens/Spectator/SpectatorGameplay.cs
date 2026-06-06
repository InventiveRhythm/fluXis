using System.Collections.Generic;
using fluXis.Database.Maps;
using fluXis.Input;
using fluXis.Mods;
using fluXis.Replays;
using fluXis.Screens.Gameplay.Replays;
using fluXis.Screens.Gameplay.Ruleset;
using osu.Framework.Input.Events;
using osu.Framework.Screens;

namespace fluXis.Screens.Spectator;

public partial class SpectatorGameplay : ReplayGameplayScreen
{
    protected override double GameplayStartTime => Replay.LastSync;

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

    protected override void LoadComplete()
    {
        base.LoadComplete();
        RulesetContainer.AllowReverting = false;
    }

    public override bool OnPressed(KeyBindingPressEvent<FluXisGlobalKeybind> e)
    {
        switch (e.Action)
        {
            case FluXisGlobalKeybind.GameplayPause:
                this.Exit();
                break;
        }

        return false;
    }
}
