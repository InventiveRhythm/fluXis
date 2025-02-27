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

    private MapInfo map { get; }

    public EditorPlaytestScreen(RealmMap realmMap, MapInfo info, double startTime, List<IMod> mods)
        : base(realmMap, mods)
    {
        GameplayStartTime = startTime;
        map = info;
    }

    protected override MapInfo LoadMap() => map;
    protected override void End() => this.Delay(GameplayClock.BeatTime * 2).FadeIn().OnComplete(_ => this.Exit());
}
