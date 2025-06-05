using System.Collections.Generic;
using fluXis.Database.Maps;
using fluXis.Map;
using fluXis.Mods;
using fluXis.Screens.Gameplay;
using osu.Framework.Graphics;
using osu.Framework.Screens;

namespace fluXis.Screens.Edit.Playtest;

public partial class EditorPlaytestScreen : GameplayScreen
{
    protected override double GameplayStartTime { get; }
    protected override bool InstantlyExitOnPause => true;
    public override bool FadeBackToGlobalClock => false;

    private EditorClock editorClock { get; }
    private MapInfo map { get; }

    public EditorPlaytestScreen(EditorClock editorClock, RealmMap realmMap, MapInfo info, double startTime, List<IMod> mods)
        : base(realmMap, mods)
    {
        this.editorClock = editorClock;
        GameplayStartTime = startTime;
        map = info;
    }

    protected override MapInfo LoadMap() => map;
    protected override void End() => this.Delay(GameplayClock.BeatTime * 2).FadeIn().OnComplete(_ => this.Exit());

    public override bool OnExiting(ScreenExitEvent e)
    {
        editorClock.Seek(GameplayClock.CurrentTime);
        editorClock.Start();
        return base.OnExiting(e);
    }
}
