using fluXis.Game.Database.Maps;
using fluXis.Game.Screens.Select.List;

namespace fluXis.Game.Tests.Select;

public partial class TestMapEntry : FluXisTestScene
{
    public TestMapEntry()
    {
        var set = new RealmMapSet();
        set.Maps.Add(new RealmMap
        {
            MapSet = set,
            Metadata = new RealmMapMetadata
            {
                Title = "Test Title",
                Artist = "Test Artist",
                Mapper = "Test Mapper",
                Source = "Test Source",
                Tags = "Test Tags",
                Background = "",
                Audio = "",
                PreviewTime = 0
            }
        });

        AddStep("Test MapEntry", () =>
        {
            ClearInternal();
            Add(new MapListEntry(null, set, 0));
        });
    }
}
