using System.Collections.Generic;
using fluXis.Configuration;
using fluXis.Database.Maps;
using fluXis.Input;
using fluXis.Mods;
using fluXis.Online.Activity;
using fluXis.Replays;
using fluXis.Screens.Gameplay.Ruleset;
using fluXis.Utils.Extensions;
using osu.Framework.Input.Events;

namespace fluXis.Screens.Gameplay.Replays;

public partial class ReplayGameplayScreen : GameplayScreen
{
    public override bool ShowCursor => true;

    protected override bool InstantlyExitOnPause => true;
    protected override bool SubmitScore => false;
    protected override bool UseGlobalOffset => !Config.Get<bool>(FluXisSetting.DisableOffsetInReplay);

    private Replay replay { get; }

    public ReplayGameplayScreen(RealmMap realmMap, List<IMod> mods, Replay replay)
        : base(realmMap, mods)
    {
        this.replay = replay;
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        RulesetContainer.AllowReverting = true;
        OnSeek += (_, now) => GameplayClock.Seek(now);
    }

    protected override RulesetContainer CreateRuleset() => new ReplayRulesetContainer(replay, Map, MapEvents, Mods) { CurrentPlayer = replay.GetPlayer(Users) };
    // protected override Drawable CreateTextOverlay() => new ReplayOverlay(replay);
    protected override UserActivity GetPlayingActivity() => new UserActivity.WatchingReplay(this, RealmMap, replay.GetPlayer(Users));

    protected override void UpdatePausedState()
    {
        base.UpdatePausedState();

        // set this back to true
        AllowOverlays.Value = true;
    }

    public override bool OnPressed(KeyBindingPressEvent<FluXisGlobalKeybind> e)
    {
        switch (e.Action)
        {
            case FluXisGlobalKeybind.ReplayPause:
                if (GameplayClock.IsRunning)
                    GameplayClock.Stop();
                else
                    GameplayClock.Start();

                return true;

            case FluXisGlobalKeybind.SeekBackward:
                GameplayClock.Seek(GameplayClock.CurrentTime - 2000);
                return true;

            case FluXisGlobalKeybind.SeekForward:
                GameplayClock.Seek(GameplayClock.CurrentTime + 2000);
                return true;
        }

        return base.OnPressed(e);
    }
}
