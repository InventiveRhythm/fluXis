using System;
using fluXis.Configuration;
using fluXis.Input;
using fluXis.Online;
using fluXis.Online.Activity;
using fluXis.Replays;
using fluXis.Scoring;
using fluXis.Screens.Gameplay.Capabilities.Bases;
using fluXis.Screens.Gameplay.Replays;
using fluXis.Screens.Gameplay.Ruleset;
using fluXis.Screens.Result;
using fluXis.Utils.Extensions;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.Screens;

namespace fluXis.Screens.Gameplay.Capabilities;

#nullable enable

public partial class ReplayCapability : Component, IRulesetCapability, IEndingCapability, IUserActivityCapability, IKeyBindingHandler<FluXisGlobalKeybind>
{
    public GameplayScreen Screen { get; set; } = null!;

    [Resolved]
    private UserCache users { get; set; } = null!;

    protected Replay Replay { get; }
    protected ReplayOverlay Overlay { get; set; } = null!;

    public ReplayCapability(Replay replay)
    {
        Replay = replay;
    }

    public virtual void PreLoad()
    {
        Screen.InstantlyExitOnPause = true;
        Screen.CursorVisible = true;
        Screen.UseGlobalOffset = !Screen.Config.Get<bool>(FluXisSetting.DisableOffsetInReplay);
    }

    public virtual void PostLoad()
    {
        Overlay = new ReplayOverlay(Replay);
        Screen.Add(Overlay);
    }

    public Screen? OnEnd(ScoreInfo score, Action complete)
    {
        Schedule(complete);
        return new Results(Screen.RealmMap, score, Replay.GetPlayer(users));
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        Screen.OnSeek += (_, now) => Screen.GameplayClock.Seek(now);
    }

    RulesetContainer IRulesetCapability.Create() => new ReplayRulesetContainer(Replay, Screen.Map, Screen.MapEvents, Screen.Mods) { CurrentPlayer = Replay.GetPlayer(users) };
    void IRulesetCapability.Modify(RulesetContainer ruleset) => ModifyRuleset((ReplayRulesetContainer)ruleset);
    protected virtual void ModifyRuleset(ReplayRulesetContainer ruleset) => ruleset.AllowReverting = true;

    UserActivity IUserActivityCapability.Create() => new UserActivity.WatchingReplay(Screen, Screen.RealmMap, Replay.GetPlayer(users));

    public virtual bool OnPressed(KeyBindingPressEvent<FluXisGlobalKeybind> e)
    {
        switch (e.Action)
        {
            case FluXisGlobalKeybind.ReplayPause:
                if (Screen.GameplayClock.IsRunning)
                    Screen.GameplayClock.Stop();
                else
                    Screen.GameplayClock.Start();

                return true;

            case FluXisGlobalKeybind.SeekBackward:
                Screen.GameplayClock.Seek(Screen.GameplayClock.CurrentTime - 2000);
                return true;

            case FluXisGlobalKeybind.SeekForward:
                Screen.GameplayClock.Seek(Screen.GameplayClock.CurrentTime + 2000);
                return true;
        }

        return false;
    }

    public void OnReleased(KeyBindingReleaseEvent<FluXisGlobalKeybind> e)
    {
    }
}
