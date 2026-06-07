using System;
using fluXis.Map;
using fluXis.Scoring;
using fluXis.Screens.Edit;
using fluXis.Screens.Gameplay.Capabilities.Bases;
using osu.Framework.Graphics;
using osu.Framework.Screens;

namespace fluXis.Screens.Gameplay.Capabilities;

public class EditorPlaytestCapability : IMapCapability, IEndingCapability
{
    public GameplayScreen Screen { get; set; }

    private readonly EditorClock clock;
    private readonly MapInfo map;
    private readonly double startTime;

    public EditorPlaytestCapability(EditorClock clock, MapInfo map, double startTime)
    {
        this.clock = clock;
        this.map = map;
        this.startTime = startTime;
    }

    void IGameplayCapability.PreLoad()
    {
        Screen.GameplayStartTime = startTime;
        Screen.FadeBackToGlobalClock = false;
        Screen.InstantlyExitOnPause = true;
    }

    void IGameplayCapability.Exit()
    {
        if (Screen.Restarting) return;

        clock.Seek(Screen.GameplayClock.CurrentTime);
        clock.Start();
    }

    Screen IEndingCapability.OnEnd(ScoreInfo score, Action complete)
    {
        Screen.Delay(Screen.GameplayClock.BeatTime * 2).FadeIn().OnComplete(_ => Screen.Exit());
        return null;
    }

    MapInfo IMapCapability.Load() => map;
}
