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

        var random = CreateDummySet(4);

        AddStep("Set MapSet", () => setMap(random.LowestDifficulty));
        AddStep("Set Lowest Difficulty", () => setMap(random.LowestDifficulty));
        AddStep("Set Highest Difficulty", () => setMap(random.HighestDifficulty));

        AddStep("Set Random MapSet", () => setMap(CreateDummySet()?.LowestDifficulty));
    }

    private void setMap(RealmMap map) => maps.CurrentMap = map;
}
