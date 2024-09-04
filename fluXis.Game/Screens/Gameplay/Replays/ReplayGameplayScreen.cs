using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Configuration;
using fluXis.Game.Configuration.Experiments;
using fluXis.Game.Database.Maps;
using fluXis.Game.Input;
using fluXis.Game.Mods;
using fluXis.Game.Online;
using fluXis.Game.Online.Activity;
using fluXis.Game.Screens.Gameplay.Input;
using fluXis.Game.Utils.Extensions;
using fluXis.Shared.Components.Users;
using fluXis.Shared.Replays;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Input.Events;

namespace fluXis.Game.Screens.Gameplay.Replays;

public partial class ReplayGameplayScreen : GameplayScreen
{
    protected override bool InstantlyExitOnPause => true;
    protected override bool AllowRestart => false;
    public override bool SubmitScore => false;
    protected override bool UseGlobalOffset => !Config.Get<bool>(FluXisSetting.DisableOffsetInReplay);
    protected override APIUser CurrentPlayer => replay.GetPlayer(users);

    [Resolved]
    private UserCache users { get; set; }

    [Resolved]
    private ExperimentConfigManager experiments { get; set; }

    private Replay replay { get; }
    private List<ReplayFrame> frames { get; }
    private Stack<ReplayFrame> handledFrames { get; }
    private List<FluXisGameplayKeybind> currentPressed = new();

    private Bindable<bool> allowSeeking;

    public ReplayGameplayScreen(RealmMap realmMap, List<IMod> mods, Replay replay)
        : base(realmMap, mods)
    {
        this.replay = replay;
        frames = replay.Frames;
        handledFrames = new Stack<ReplayFrame>();
    }

    protected override GameplayInput GetInput() => new ReplayInput(this, RealmMap.KeyCount);
    protected override Drawable CreateTextOverlay() => new ReplayOverlay(replay);
    protected override UserActivity GetPlayingActivity() => new UserActivity.WatchingReplay(this, RealmMap, replay.GetPlayer(users));

    protected override void UpdatePausedState()
    {
        base.UpdatePausedState();

        // set this back to true
        AllowOverlays.Value = true;
    }

    protected override void Update()
    {
        base.Update();

        if (frames.Count == 0)
            return;

        while (frames.Count > 0 && frames[0].Time <= GameplayClock.CurrentTime)
        {
            var frame = frames[0];
            frames.RemoveAt(0);
            handledFrames.Push(frame);
            handlePresses(frame.Actions);
        }

        while (handledFrames.Count > 0)
        {
            var result = handledFrames.Peek();

            if (GameplayClock.CurrentTime >= result.Time)
                break;

            revertFrame(handledFrames.Pop());
        }
    }

    private void revertFrame(ReplayFrame frame)
    {
        foreach (var keybind in currentPressed)
            Input.ReleaseKey(keybind);

        currentPressed.Clear();
        frames.Insert(0, frame);
    }

    private void handlePresses(List<int> frameActionsInt)
    {
        var frameActions = frameActionsInt.Select(i => (FluXisGameplayKeybind)i).ToList();

        foreach (var keybind in frameActions)
        {
            if (currentPressed.Contains(keybind))
                continue;

            Input.PressKey(keybind);
        }

        foreach (var keybind in currentPressed)
        {
            if (frameActions.Contains(keybind))
                continue;

            Input.ReleaseKey(keybind);
        }

        currentPressed = frameActions;
    }

    public override bool OnPressed(KeyBindingPressEvent<FluXisGlobalKeybind> e)
    {
        allowSeeking ??= experiments.GetBindable<bool>(ExperimentConfig.Seeking);

        if (!allowSeeking.Value)
            return base.OnPressed(e);

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
