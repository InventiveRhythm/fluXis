using fluXis.Game.Database.Maps;
using fluXis.Game.Map;
using fluXis.Game.Scoring;
using fluXis.Game.Screens.Result;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Screens;

namespace fluXis.Game.Tests.Results;

public partial class TestResultsScreen : FluXisTestScene
{
    [Resolved]
    private MapStore mapStore { get; set; }

    public TestResultsScreen()
    {
        var screenStack = new ScreenStack
        {
            RelativeSizeAxes = Axes.Both
        };

        Add(screenStack);

        AddStep("Show results screen", () =>
        {
            RealmMap realmMap;

            if (mapStore.MapSets.Count > 0)
                realmMap = mapStore.MapSets[0].Maps[0];
            else
            {
                realmMap = new RealmMap
                {
                    ID = default,
                    Difficulty = "Difficulty Name",
                    Metadata = new RealmMapMetadata
                    {
                        Title = "Title",
                        Artist = "Artist",
                        Mapper = "Mapper"
                    },
                    MapSet = null,
                    Status = 0,
                    OnlineID = 0,
                    Hash = null,
                    KeyCount = 0,
                    Rating = 0
                };
            }

            var map = new MapInfo(new MapMetadata
            {
                Title = realmMap.Metadata.Title,
                Artist = realmMap.Metadata.Artist,
                Mapper = realmMap.Metadata.Mapper
            });

            screenStack.Push(new ResultsScreen(realmMap, map, new Performance(map, 0, ""), false));
        });
    }
}
