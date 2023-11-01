using System.Collections.Generic;
using fluXis.Game.Graphics.Background;
using fluXis.Game.Map;
using fluXis.Game.Online.API;
using fluXis.Game.Online.API.Models.Scores;
using fluXis.Game.Online.API.Models.Users;
using fluXis.Game.Online.API.Requests.Scores;
using fluXis.Game.Overlay.Toolbar;
using fluXis.Game.Scoring;
using fluXis.Game.Scoring.Enums;
using fluXis.Game.Screens;
using fluXis.Game.Screens.Result;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Game.Tests.Results;

public partial class TestNewResults : FluXisTestScene
{
    [BackgroundDependencyLoader]
    private void load(MapStore maps, BackgroundStack backgrounds, Toolbar toolbar)
    {
        var score = new ScoreInfo
        {
            Accuracy = 98.661736f,
            Rank = ScoreRank.S,
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

        var stack = new FluXisScreenStack();

        AddRange(new Drawable[]
        {
            backgrounds,
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                Padding = new MarginPadding { Top = 60 },
                Child = stack
            },
            toolbar
        });

        var set = maps.GetFromGuid("9896365c-5541-4612-9f39-5a44aa1012ed");
        var map = set?.Maps[0] ?? maps.MapSets[0].Maps[0];

        var screen = new SoloResults(map, score, APIUserShort.Dummy, false);
        screen.SubmitRequest = new SimulatedScoreRequest(score);
        stack.Push(screen);
    }

    private class SimulatedScoreRequest : ScoreSubmitRequest
    {
        public SimulatedScoreRequest(ScoreInfo score)
            : base(score)
        {
            Response = new APIResponse<APIScoreResponse>(200, "", new APIScoreResponse
            {
                OverallRating = 10,
                PotentialRating = 10,
                Rank = 1,
                OverallRatingChange = 2,
                PotentialRatingChange = 2,
                RankChange = 1
            });
        }
    }
}
