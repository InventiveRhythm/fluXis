using System.Collections.Generic;
using fluXis.Game.Database.Maps;
using fluXis.Game.Map;
using fluXis.Game.Online.API;
using fluXis.Game.Online.API.Requests.Scores;
using fluXis.Game.Screens;
using fluXis.Game.Screens.Result;
using fluXis.Shared.API.Responses.Scores;
using fluXis.Shared.Components.Scores;
using fluXis.Shared.Components.Users;
using fluXis.Shared.Scoring;
using fluXis.Shared.Scoring.Enums;
using NUnit.Framework;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Logging;

namespace fluXis.Game.Tests.Screens;

public partial class TestResults : FluXisTestScene
{
    [Resolved]
    private MapStore maps { get; set; }

    private FluXisScreenStack stack;

    [BackgroundDependencyLoader]
    private void load()
    {
        CreateClock();
    }

    [SetUp]
    public void Setup() => Schedule(() =>
    {
        Children = new Drawable[]
        {
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                Padding = new MarginPadding { Top = 60 },
                Child = stack = new FluXisScreenStack()
            }
        };
    });

    private RealmMap getMap()
    {
        var set = maps.GetFromGuid("9896365c-5541-4612-9f39-5a44aa1012ed");
        return set?.Maps[0] ?? maps.MapSets[0].Maps[0];
    }

    private ScoreInfo getScore() => new()
    {
        Accuracy = 98.661736f,
        Rank = ScoreRank.S,
        PerformanceRating = 8,
        Score = 1139289,
        MaxCombo = 1218,
        Flawless = 898,
        Perfect = 290,
        Great = 30,
        Alright = 0,
        Okay = 0,
        Miss = 0,
        Mods = new List<string> { "1.5x" }
    };

    [Test]
    public void FromGameplay()
    {
        AddStep("Push", () => stack.Push(new SoloResults(getMap(), getScore(), APIUser.Dummy) { ForceFromGameplay = true }));
    }

    [Test]
    public void Normal()
    {
        AddStep("Push", () => stack.Push(new SoloResults(getMap(), getScore(), APIUser.Dummy)));
    }

    [Test]
    public void WithRequest()
    {
        var score = getScore();
        AddStep("Push With Request", () => stack.Push(new SoloResults(getMap(), score, APIUser.Dummy) { SubmitRequest = new SimulatedScoreRequest(score) }));
    }

    [Test]
    public void WithRestart()
    {
        AddStep("Push With Restart", () => stack.Push(new SoloResults(getMap(), getScore(), APIUser.Dummy) { OnRestart = () => Logger.Log("Restart pressed.") }));
    }

    private class SimulatedScoreRequest : ScoreSubmitRequest
    {
        public SimulatedScoreRequest(ScoreInfo score)
            : base(score)
        {
            Response = new APIResponse<ScoreSubmissionStats>(200, "", new ScoreSubmissionStats(new APIScore { PerformanceRating = 7 }, 10, 10, 1, 12, 10, 2));
        }
    }
}
