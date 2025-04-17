using System.Collections.Generic;
using fluXis.Database.Maps;
using fluXis.Map;
using fluXis.Mods;
using fluXis.Replays;
using fluXis.Screens.Gameplay.Replays;
using osu.Framework.Graphics;
using osu.Framework.Screens;

namespace fluXis.Screens.Edit.Playtest;

public partial class EditorAutoPlaytestScreen : ReplayGameplayScreen
{
    protected override double GameplayStartTime { get; }
    public override bool FadeBackToGlobalClock => false;

    private MapInfo map { get; }

    public EditorAutoPlaytestScreen(RealmMap realmMap, MapInfo info, double startTime, List<IMod> mods)
        : base(realmMap, mods, new AutoGenerator(info, realmMap.KeyCount).Generate())
    {
        GameplayStartTime = startTime;
        map = info;
    }

    protected override MapInfo LoadMap() => map;
    public override void OnDeath() => this.Exit();
    protected override void End() => this.Delay(GameplayClock.BeatTime * 2).FadeIn().OnComplete(_ => this.Exit());
}
