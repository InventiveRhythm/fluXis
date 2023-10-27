using fluXis.Game.Online.API;
using fluXis.Game.Online.API.Models.Scores;
using fluXis.Game.Screens.Result.UI;
using osu.Framework.Allocation;

namespace fluXis.Game.Tests.Results;

public partial class TestResultsRating : FluXisTestScene
{
    [BackgroundDependencyLoader]
    private void load()
    {
        var ratingInfo = new ResultsRatingInfo(true);
        Child = ratingInfo;

        AddStep("Unchanged", () => ratingInfo.ScoreResponse = new APIResponse<APIScoreResponse>(200, "", new APIScoreResponse
        {
            OverallRating = 10,
            PotentialRating = 10,
            OverallRatingChange = 0,
            PotentialRatingChange = 0
        }));

        AddStep("Higher", () => ratingInfo.ScoreResponse = new APIResponse<APIScoreResponse>(200, "", new APIScoreResponse
        {
            OverallRating = 10,
            PotentialRating = 10,
            OverallRatingChange = 10,
            PotentialRatingChange = 10
        }));

        AddStep("Lower", () => ratingInfo.ScoreResponse = new APIResponse<APIScoreResponse>(200, "", new APIScoreResponse
        {
            OverallRating = 10,
            PotentialRating = 10,
            OverallRatingChange = -10,
            PotentialRatingChange = -10
        }));

        AddStep("Error", () => ratingInfo.ScoreResponse = new APIResponse<APIScoreResponse>(500, "Something went wrong.", null));
    }
}
