using System.Collections.Generic;
using fluXis.Graphics.Background;
using fluXis.Map;
using fluXis.Scoring;
using fluXis.Scoring.Enums;
using fluXis.Screens;
using fluXis.Screens.Multiplayer.Gameplay;
using osu.Framework.Allocation;

namespace fluXis.Tests.Multiplayer;

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
                Mods = new List<string>(),
                MapID = map.OnlineID,
                Players = new List<PlayerScore>
                {
                    new PlayerScore
                    {
                        Accuracy = 99f,
                        Combo = 100,
                        MaxCombo = 100,
                        Rank = ScoreRank.SS,
                        Score = 1000000,
                        Flawless = 100,
                        Perfect = 100,
                        Great = 0,
                        Alright = 0,
                        Okay = 0,
                        Miss = 0,
                        PlayerID = 1,
                    }
                }
            },
            new()
            {
                Mods = new List<string>(),
                MapID = map.OnlineID,
                Players = new List<PlayerScore>
                {
                    new PlayerScore
                    {
                        Accuracy = 100f,
                        Combo = 100,
                        MaxCombo = 100,
                        Rank = ScoreRank.X,
                        Score = 1100000,
                        Flawless = 100,
                        Perfect = 100,
                        Great = 0,
                        Alright = 0,
                        Okay = 0,
                        Miss = 0,
                        PlayerID = 2,
                    }
                }
            }
        };

        AddStep("Push Results", () => stack.Push(new MultiResults(map, scores)));
    }
}
