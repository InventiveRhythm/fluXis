using fluXis.Online.Spectator;
using fluXis.Replays;
using osu.Framework.Allocation;
using osu.Framework.Graphics;

namespace fluXis.Screens.Gameplay.Capabilities;

#nullable enable

public partial class SpectatorSendCapability : Component, IGameplayCapability
{
    public GameplayScreen Screen { get; set; } = null!;

    [Resolved(CanBeNull = true)]
    private SpectatorClient? spectator { get; set; }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        if (Screen.RealmMap.OnlineID > 0 && spectator != null)
        {
            spectator.StartPlaying(Screen.RealmMap.OnlineID, Screen.Mods);
            Screen.ReplayRecorder.OnFrameCreated += onFrameRecorded;
        }
    }

    protected override void Update()
    {
        base.Update();
        spectator?.UpdateTime(Screen.GameplayClock.CurrentTime);
    }

    public void Exit()
    {
        spectator?.StopPlaying();
        Screen.ReplayRecorder.OnFrameCreated -= onFrameRecorded;
    }

    private void onFrameRecorded(ReplayFrame frame) => spectator?.BufferFrame(frame);
}
