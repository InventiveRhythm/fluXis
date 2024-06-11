using fluXis.Game.Online.API;
using fluXis.Game.Screens.Result.UI;
using fluXis.Shared.API.Responses.Scores;
using osu.Framework.Allocation;

namespace fluXis.Game.Tests.Results;

public partial class TestResultsRating : FluXisTestScene
{
    [BackgroundDependencyLoader]
    private void load()
    {
        var ratingInfo = new ResultsRatingInfo(true);
        Child = ratingInfo;

        AddStep("Unchanged", () => ratingInfo.ScoreResponse = new APIResponse<ScoreSubmissionStats>(200, "", new ScoreSubmissionStats(0, 10, 10, 1, 0, 0, 0)));
        AddStep("Higher", () => ratingInfo.ScoreResponse = new APIResponse<ScoreSubmissionStats>(200, "", new ScoreSubmissionStats(0, 10, 10, 1, 10, 10, 1)));
        AddStep("Lower", () => ratingInfo.ScoreResponse = new APIResponse<ScoreSubmissionStats>(200, "", new ScoreSubmissionStats(0, 10, 10, 1, -10, -10, -1)));
        AddStep("Error", () => ratingInfo.ScoreResponse = new APIResponse<ScoreSubmissionStats>(500, "Something went wrong.", null));
    }
}
