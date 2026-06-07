using fluXis.Input;
using fluXis.Replays;
using fluXis.Screens.Gameplay.Replays;
using osu.Framework.Input.Events;
using osu.Framework.Screens;

namespace fluXis.Screens.Gameplay.Capabilities;

#nullable enable

public partial class SpectatorCapability : ReplayCapability
{
    public SpectatorCapability(Replay replay)
        : base(replay)
    {
    }

    public override void PreLoad()
    {
        base.PreLoad();
        Screen.GameplayStartTime = Replay.LastSync;
    }

    public override void PostLoad()
    {
        base.PostLoad();
        Overlay.Title = "Spectator Mode";
        Overlay.SubTitle = u => $"Watching {u?.Username}";
    }

    protected override void ModifyRuleset(ReplayRulesetContainer ruleset)
    {
        ruleset.AllowReverting = false;
        ruleset.RequireSyncFrames = true;
    }

    public override bool OnPressed(KeyBindingPressEvent<FluXisGlobalKeybind> e)
    {
        switch (e.Action)
        {
            case FluXisGlobalKeybind.GameplayPause:
                Screen.Exit();
                return true;
        }

        return false;
    }
}
