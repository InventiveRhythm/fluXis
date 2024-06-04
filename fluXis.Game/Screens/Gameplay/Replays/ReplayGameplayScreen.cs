using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Configuration;
using fluXis.Game.Database.Maps;
using fluXis.Game.Input;
using fluXis.Game.Mods;
using fluXis.Game.Online.Activity;
using fluXis.Game.Screens.Gameplay.Input;
using fluXis.Game.Utils.Extensions;
using fluXis.Shared.Replays;
using osu.Framework.Graphics;
using osu.Framework.Input.Events;

namespace fluXis.Game.Screens.Gameplay.Replays;

public partial class ReplayGameplayScreen : GameplayScreen
{
    protected override bool InstantlyExitOnPause => true;
    public override bool SubmitScore => false;
    protected override bool UseGlobalOffset => !Config.Get<bool>(FluXisSetting.DisableOffsetInReplay);

    private Replay replay { get; }
    private List<ReplayFrame> frames { get; }
    private List<FluXisGameplayKeybind> currentPressed = new();

    public ReplayGameplayScreen(RealmMap realmMap, List<IMod> mods, Replay replay)
        : base(realmMap, mods)
    {
        this.replay = replay;
        frames = replay.Frames;
    }

    protected override GameplayInput GetInput() => new ReplayInput(this, RealmMap.KeyCount);
    protected override Drawable CreateTextOverlay() => new ReplayOverlay(replay);
    protected override UserActivity GetPlayingActivity() => new UserActivity.WatchingReplay(RealmMap, replay.GetPlayer());

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
            handlePresses(frame.Actions);
        }
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
        switch (e.Action)
        {
            case FluXisGlobalKeybind.SeekBackward:
                OnSeek?.Invoke(GameplayClock.CurrentTime, GameplayClock.CurrentTime - 5000);
                return true;

            case FluXisGlobalKeybind.SeekForward:
                OnSeek?.Invoke(GameplayClock.CurrentTime, GameplayClock.CurrentTime + 5000);
                return true;
        }

        return base.OnPressed(e);
    }
}
