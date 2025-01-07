using System.Linq;
using fluXis.Database.Maps;
using fluXis.Map;
using fluXis.Screens.Select.Info;
using osu.Framework.Allocation;

namespace fluXis.Tests.Select;

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
