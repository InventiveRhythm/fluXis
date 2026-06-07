using System;
using System.Collections.Generic;
using fluXis.Database.Maps;
using fluXis.Graphics.Background;
using fluXis.Map;
using fluXis.Mods;
using fluXis.Online.API;
using fluXis.Online.API.Models.Scores;
using fluXis.Online.API.Models.Users;
using fluXis.Online.API.Requests.Scores;
using fluXis.Online.API.Responses.Scores;
using fluXis.Replays;
using fluXis.Scoring;
using fluXis.Scoring.Enums;
using fluXis.Screens;
using fluXis.Screens.Multiplayer.Gameplay;
using fluXis.Screens.Result;
using NUnit.Framework;
using osu.Framework.Allocation;
using osu.Framework.Logging;
using osu.Framework.Utils;

namespace fluXis.Tests.Screens;

public partial class TestResults : FluXisTestScene
{
    [Resolved]
    private MapStore maps { get; set; }

    private FluXisScreenStack stack;

    [BackgroundDependencyLoader]
    private void load()
    {
        CreateClock();
        TestDependencies.CacheAs(new GlobalBackground());
    }

    [SetUp]
    public void Setup() => Schedule(() =>
    {
        Child = stack = new FluXisScreenStack();
    });

    private RealmMap getMap()
    {
        var set = maps.GetFromGuid("9896365c-5541-4612-9f39-5a44aa1012ed");
        return set?.Maps[0] ?? maps.MapSets[0].Maps[0];
    }

    private ScoreInfo getScore(long player = -1) => new()
    {
        Accuracy = RNG.NextSingle(80, 100),
        Rank = (ScoreRank)RNG.Next(Enum.GetValues<ScoreRank>().Length),
        PerformanceRating = RNG.NextSingle(4, 200),
        Score = RNG.Next(800000, 1200000),
        MaxCombo = RNG.Next(1219),
        Flawless = RNG.Next(898),
        Perfect = RNG.Next(290),
        Great = RNG.Next(100),
        Alright = RNG.Next(100),
        Okay = RNG.Next(100),
        Miss = RNG.Next(100),
        Mods = new List<string> { "1.5x" },
        PlayerID = player
    };

    [Test]
    public void Normal()
    {
        AddStep("Push", () => stack.Push(new Results(getMap(), getScore(), APIUser.Dummy)));
    }

    [Test]
    public void WithRequestFromGameplay()
    {
        var score = getScore();
        AddStep("Push With Request", () => stack.Push(new SoloResults(getMap(), score, APIUser.Dummy)
        {
            SubmitRequest = new SimulatedScoreRequest(score, new List<IMod>(), new Replay(), "", "", "")
        }));
    }

    [Test]
    public void WithRestart()
    {
        AddStep("Push With Restart", () => stack.Push(new Results(getMap(), getScore(), APIUser.Dummy) { OnRestart = () => Logger.Log("Restart pressed.") }));
    }

    [Test]
    public void Multiplayer()
    {
        MultiplayerClient.Join(0).Wait();
        MultiplayerClient.AddPlayer(APIUser.CreateUnknown(1));

        AddStep("Push", () => stack.Push(new MultiplayerResults(getMap(), [getScore(), getScore(1)], MultiplayerClient)));
    }

    private class SimulatedScoreRequest : ScoreSubmitRequest
    {
        public SimulatedScoreRequest(ScoreInfo score, List<IMod> mods, Replay replay, string hash, string eHash, string sHash)
            : base(score, mods, replay, hash, eHash, sHash)
        {
            Response = new APIResponse<ScoreSubmissionStats>(200, "", new ScoreSubmissionStats(new APIScore { PerformanceRating = 7 }, 10, 10, 1, 12, 10, 2));
        }
    }
}
