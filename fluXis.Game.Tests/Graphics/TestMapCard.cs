using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Map.Drawables.Card;
using fluXis.Game.Overlay.Mouse;
using fluXis.Shared.Components.Maps;
using fluXis.Shared.Components.Users;
using NUnit.Framework;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Game.Tests.Graphics;

public partial class TestMapCard : FluXisTestScene
{
    private FillFlowContainer container;

    [Resolved]
    private GlobalCursorOverlay cursor { get; set; }

    [SetUp]
    public void Setup() => Schedule(() =>
    {
        Child = new GlobalTooltipContainer(cursor.Cursor)
        {
            RelativeSizeAxes = Axes.Both,
            Child = container = new FillFlowContainer()
            {
                AutoSizeAxes = Axes.Both,
                Direction = FillDirection.Vertical,
                Spacing = new Vector2(10),
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre
            }
        };
    });

    private APIMapSet create(Action<APIMapSet> func = null)
    {
        var set = new APIMapSet()
        {
            ID = 1,
            Creator = APIUser.CreateUnknown(1),
            Title = "super cool song with a long name",
            Artist = "super cool artist",
            Source = "somewhere",
            Tags = new[] { "some", "cool", "and fitting", "tags" },
            Maps = new List<APIMap>
            {
                new()
                {
                    ID = 1,
                    MapSetID = 1,
                    Mapper = APIUser.CreateUnknown(1),
                    SHA256Hash = "veryvalidhash",
                    Difficulty = "hard",
                    Title = "super cool song",
                    Artist = "super cool artist",
                    Source = "somewhere",
                    Tags = "some, cool, and fitting, tags",
                    Mode = 4,
                    Status = 0,
                    BPM = 120,
                    Length = 12000,
                    NoteCount = 456,
                    LongNoteCount = 32,
                    MaxCombo = 488,
                    NotesPerSecond = 14.24,
                    Rating = 8,
                    UpVotes = 4,
                    DownVotes = 6,
                    FileName = "195769352.fsc"
                }
            }
        };

        func?.Invoke(set);
        return set;
    }

    [Test]
    public void TestDefault()
    {
        AddStep("add default", () => container.Child = new MapCard(create()));
    }

    [Test]
    public void TestFlags()
    {
        AddStep("add normal", () => container.Add(new MapCard(create())));
        AddStep("add explicit", () => container.Add(new MapCard(create(s => s.Flags |= MapSetFlag.Explicit))));
        AddStep("add featured", () => container.Add(new MapCard(create(s => s.Flags |= MapSetFlag.FeaturedArtist))));
    }

    [Test]
    public void TestRankStatus()
    {
        var states = Enumerable.Range(0, 4);
        AddStep("add all", () =>
        {
            foreach (var state in states)
                container.Add(new MapCard(create(s => s.Status = state)));
        });
    }

    [Test]
    public void TestNullData()
    {
        AddStep("add null", () => container.Child = new MapCard(null));
    }
}
