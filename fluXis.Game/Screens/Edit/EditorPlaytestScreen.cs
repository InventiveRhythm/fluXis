using System.Collections.Generic;
using fluXis.Game.Database.Maps;
using fluXis.Game.Map;
using fluXis.Game.Mods;
using fluXis.Game.Screens.Gameplay;
using osu.Framework.Screens;

namespace fluXis.Game.Screens.Edit;

public partial class EditorPlaytestScreen : GameplayScreen
{
    protected override double GameplayStartTime { get; }
    protected override bool InstantlyExitOnPause => true;
    public override bool FadeBackToGlobalClock => false;

    private readonly MapInfo map;
    private readonly MapEvents events;

    public EditorPlaytestScreen(RealmMap realmMap, MapInfo info, MapEvents events, double startTime)
        : base(realmMap, new List<IMod> { new NoFailMod() })
    {
        map = info;
        this.events = events;
        GameplayStartTime = startTime;
    }

    protected override MapInfo LoadMap()
    {
        return map;
    }

    public override bool OnExiting(ScreenExitEvent e)
    {
        base.OnExiting(e);
        return false;
    }

    public override void RestartMap()
    {
        Samples.Restart();
        this.Push(new EditorPlaytestScreen(RealmMap, map, events, GameplayStartTime));
    }

    public override void OnDeath()
    {
        GameplayClock.Stop();
        this.Exit();
    }

    protected override void End()
    {
        GameplayClock.Stop();
        this.Exit();
    }
}
