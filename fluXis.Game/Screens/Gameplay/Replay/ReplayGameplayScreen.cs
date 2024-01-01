using System.Collections.Generic;
using fluXis.Game.Database.Maps;
using fluXis.Game.Input;
using fluXis.Game.Mods;
using fluXis.Game.Online.Activity;
using fluXis.Game.Replays;
using fluXis.Game.Screens.Gameplay.Input;
using osu.Framework.Graphics;

namespace fluXis.Game.Screens.Gameplay.Replay;

public partial class ReplayGameplayScreen : GameplayScreen
{
    protected override bool InstantlyExitOnPause => true;
    public override bool SubmitScore => false;

    private Replays.Replay replay { get; }
    private List<ReplayFrame> frames { get; }
    private List<FluXisGameplayKeybind> currentPressed = new();

    public ReplayGameplayScreen(RealmMap realmMap, List<IMod> mods, Replays.Replay replay)
        : base(realmMap, mods)
    {
        this.replay = replay;
        frames = replay.Frames;
    }

    protected override GameplayInput GetInput() => new ReplayInput(this, RealmMap.KeyCount);
    protected override Drawable CreateTextOverlay() => new ReplayOverlay(replay);
    protected override UserActivity GetPlayingActivity() => new UserActivity.WatchingReplay(RealmMap, replay.Player);

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

    private void handlePresses(List<FluXisGameplayKeybind> frameActions)
    {
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
}
