using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Game.Online.API;
using fluXis.Game.Online.API.Models.Maps;
using fluXis.Game.Online.API.Models.Users;
using fluXis.Game.Online.API.Requests.MapSets;
using fluXis.Game.Screens;
using fluXis.Game.Screens.Browse;
using osu.Framework.Allocation;
using osu.Framework.Extensions.IEnumerableExtensions;

namespace fluXis.Game.Tests.Screens;

public partial class TestMapBrowser : FluXisTestScene
{
    protected override bool UseTestAPI => true;

    [BackgroundDependencyLoader]
    private void load()
    {
        CreateClock();

        TestAPI.HandleRequest += handleApiCall;

        var stack = new FluXisScreenStack();
        TestDependencies.Cache(stack);
        Add(stack);

        LoadComponent(GlobalClock);

        AddStep("Push Screen", () => stack.Push(new MapBrowser()));
    }

    private int count;

    private void handleApiCall(APIRequest request)
    {
        if (request is MapSetsRequest setsRequest)
        {
            var sets = new List<APIMapSet>();

            if (count < 200)
            {
                Enumerable.Range(count, 50).Select(x => create(s =>
                {
                    s.ID = 0;
                    s.Title = $"set {x + 1}";
                })).ForEach(sets.Add);
                count += 50;
            }

            var res = new APIResponse<List<APIMapSet>>(200, "", sets);
            setsRequest.TriggerSuccess(res);
        }
    }

    private APIMapSet create(Action<APIMapSet> func = null)
    {
        var set = new APIMapSet
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
}
