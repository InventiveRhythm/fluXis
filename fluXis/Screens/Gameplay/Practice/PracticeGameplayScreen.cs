using System.Collections.Generic;
using System.Linq;
using fluXis.Database.Maps;
using fluXis.Map;
using fluXis.Mods;

namespace fluXis.Screens.Gameplay.Practice;

public partial class PracticeGameplayScreen : GameplayScreen
{
    protected override bool SubmitScore => false;
    protected override double GameplayStartTime => start;

    private float start { get; }
    private float end { get; }

    public PracticeGameplayScreen(RealmMap realmMap, List<IMod> mods, float start, float end)
        : base(realmMap, mods)
    {
        this.start = start;
        this.end = end;
    }

    protected override MapInfo LoadMap()
    {
        var map = base.LoadMap();

        if (map is null)
            return null;

        map.HitObjects = map.HitObjects.Where(o => o.Time >= start && o.Time <= end).ToList();
        return map;
    }
}
