using System.Collections.Generic;
using fluXis.Game.Database.Maps;
using fluXis.Game.Map;
using fluXis.Game.Mods;
using fluXis.Game.Screens.Gameplay;
using osu.Framework.Graphics;
using osu.Framework.Screens;

namespace fluXis.Game.Screens.Edit.Playtest;

public partial class EditorPlaytestScreen : GameplayScreen
{
    protected override double GameplayStartTime { get; }
    protected override bool InstantlyExitOnPause => true;
    public override bool FadeBackToGlobalClock => false;

    private MapInfo map { get; }

    public EditorPlaytestScreen(RealmMap realmMap, MapInfo info, double startTime)
        : base(realmMap, new List<IMod> { new NoFailMod() })
    {
        GameplayStartTime = startTime;
        map = info;
    }

    protected override MapInfo LoadMap() => map;
    protected override void End() => this.Delay(GameplayClock.BeatTime * 2).FadeIn().OnComplete(_ => this.Exit());
}
