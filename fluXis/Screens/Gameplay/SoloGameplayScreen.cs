using System.Collections.Generic;
using fluXis.Database.Maps;
using fluXis.Mods;
using fluXis.Online.Spectator;
using fluXis.Replays;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Screens;

namespace fluXis.Screens.Gameplay;

public partial class SoloGameplayScreen : GameplayScreen
{
    [CanBeNull]
    [Resolved(CanBeNull = true)]
    private SpectatorClient spectator { get; set; }

    public SoloGameplayScreen(RealmMap realmMap, List<IMod> mods)
        : base(realmMap, mods)
    {
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        if (RealmMap.OnlineID > 0 && spectator != null)
        {
            spectator.StartPlaying(RealmMap.OnlineID, Mods);
            ReplayRecorder.OnFrameCreated += onFrameRecorded;
        }
    }

    protected override void Update()
    {
        base.Update();

        spectator?.UpdateTime(GameplayClock.CurrentTime);
    }

    public override bool OnExiting(ScreenExitEvent e)
    {
        spectator?.StopPlaying();
        ReplayRecorder.OnFrameCreated -= onFrameRecorded;

        return base.OnExiting(e);
    }

    private void onFrameRecorded(ReplayFrame frame) => spectator?.BufferFrame(frame);
}
