using System.Collections.Generic;
using fluXis.Game.Graphics.Background;
using fluXis.Game.Map;
using fluXis.Game.Scoring;
using fluXis.Game.Scoring.Enums;
using fluXis.Game.Screens;
using fluXis.Game.Screens.Multiplayer.Gameplay;
using osu.Framework.Allocation;

namespace fluXis.Game.Tests.Multiplayer;

public partial class TestMultiResults : FluXisTestScene
{
    [BackgroundDependencyLoader]
    private void load(MapStore store)
    {
        CreateClock();

        var background = new GlobalBackground();
        TestDependencies.Cache(background);
        Add(background);

        var stack = new FluXisScreenStack();
        Add(stack);

        var map = store.GetRandom()!.LowestDifficulty!;

        var scores = new List<ScoreInfo>
        {
            new()
            {
                Accuracy = 99f,
                Combo = 100,
                MaxCombo = 100,
                Mods = new List<string>(),
                Rank = ScoreRank.SS,
                Score = 1000000,
                Flawless = 100,
                Perfect = 100,
                Great = 0,
                Alright = 0,
                Okay = 0,
                Miss = 0,
                PlayerID = 1,
                MapID = map.OnlineID
            },
            new()
            {
                Accuracy = 100f,
                Combo = 100,
                MaxCombo = 100,
                Mods = new List<string>(),
                Rank = ScoreRank.X,
                Score = 1100000,
                Flawless = 100,
                Perfect = 100,
                Great = 0,
                Alright = 0,
                Okay = 0,
                Miss = 0,
                PlayerID = 2,
                MapID = map.OnlineID
            }
        };

        AddStep("Push Results", () => stack.Push(new MultiResults(map, scores)));
    }
}
