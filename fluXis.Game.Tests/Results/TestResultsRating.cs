using fluXis.Game.Online.API;
using fluXis.Game.Screens.Result.UI;
using fluXis.Shared.API.Responses.Scores;
using fluXis.Shared.Components.Scores;
using osu.Framework.Allocation;

namespace fluXis.Game.Tests.Results;

public partial class TestResultsRating : FluXisTestScene
{
    [BackgroundDependencyLoader]
    private void load()
    {
        var ratingInfo = new ResultsRatingInfo(true);
        Child = ratingInfo;

        AddStep("Unchanged", () => ratingInfo.ScoreResponse = new APIResponse<ScoreSubmissionStats>(200, "", new ScoreSubmissionStats(new APIScore(), 10, 10, 1, 10, 10, 1)));
        AddStep("Higher", () => ratingInfo.ScoreResponse = new APIResponse<ScoreSubmissionStats>(200, "", new ScoreSubmissionStats(new APIScore(), 10, 10, 2, 20, 20, 1)));
        AddStep("Lower", () => ratingInfo.ScoreResponse = new APIResponse<ScoreSubmissionStats>(200, "", new ScoreSubmissionStats(new APIScore(), 10, 10, 1, 5, 5, 2)));
        AddStep("Error", () => ratingInfo.ScoreResponse = new APIResponse<ScoreSubmissionStats>(500, "Something went wrong.", null));
    }
}
