using System.Linq;
using fluXis.Game.Database.Maps;
using fluXis.Game.Map;
using fluXis.Game.Screens.Select.Info;
using osu.Framework.Allocation;

namespace fluXis.Game.Tests.Select;

public partial class TestMapInfo : FluXisTestScene
{
    [Resolved]
    private MapStore maps { get; set; }

    [BackgroundDependencyLoader]
    private void load()
    {
        CreateDummyBeatSync();

        var info = new SelectMapInfo();
        Add(info);

        AddStep("Set MapSet", () => setMap(maps.MapSets.First().LowestDifficulty));
        AddStep("Set Lowest Difficulty", () => setMap(maps.MapSets.First().LowestDifficulty));
        AddStep("Set Highest Difficulty", () => setMap(maps.MapSets.First().HighestDifficulty));

        AddStep("Set Random MapSet", () => setMap(maps.GetRandom()?.LowestDifficulty));
    }

    private void setMap(RealmMap map) => maps.CurrentMap = map;
}
