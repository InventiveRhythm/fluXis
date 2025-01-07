using System.Collections.Generic;
using System.Linq;
using fluXis.Database.Maps;
using fluXis.Screens.Select.List.Drawables.MapSet;
using fluXis.Screens.Select.List.Items;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Tests.Select;

public partial class TestMapEntry : FluXisTestScene
{
    public TestMapEntry()
    {
        var maps = new List<RealmMapSet>
        {
            new(new List<RealmMap>
            {
                new()
                {
                    Difficulty = "Easy",
                    KeyCount = 4,
                    Metadata = new RealmMapMetadata
                    {
                        Title = "Test 1",
                        Artist = "Artist",
                        Mapper = "Mapper 1"
                    },
                    Filters = new RealmMapFilters { NotesPerSecond = 5 }
                },
                new()
                {
                    Difficulty = "Hard",
                    KeyCount = 6,
                    Metadata = new RealmMapMetadata
                    {
                        Title = "Test 1",
                        Artist = "Artist",
                        Mapper = "Mapper 1"
                    },
                    Filters = new RealmMapFilters { NotesPerSecond = 14 }
                }
            }),
            new(new List<RealmMap>
            {
                new()
                {
                    Difficulty = "Expert",
                    KeyCount = 5,
                    Metadata = new RealmMapMetadata
                    {
                        Title = "Test 2",
                        Artist = "Artist",
                        Mapper = "Mapper 2"
                    },
                    Filters = new RealmMapFilters { NotesPerSecond = 22 }
                },
                new()
                {
                    Difficulty = "Master",
                    KeyCount = 8,
                    Metadata = new RealmMapMetadata
                    {
                        Title = "Test 2",
                        Artist = "Artist",
                        Mapper = "Mapper 2"
                    },
                    Filters = new RealmMapFilters { NotesPerSecond = 26 }
                }
            })
        };

        AddStep("Test MapEntry", () =>
        {
            Clear();
            Add(new FillFlowContainer
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Width = 0.7f,
                Direction = FillDirection.Vertical,
                Spacing = new Vector2(10),
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                ChildrenEnumerable = maps.Select(set => new DrawableMapSetItem(new MapSetItem(set), set))
            });
        });
    }
}
