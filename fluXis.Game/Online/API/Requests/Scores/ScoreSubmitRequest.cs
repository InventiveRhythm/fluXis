using System.Net.Http;
using fluXis.Game.Online.API.Models.Scores;
using fluXis.Shared.Scoring;
using fluXis.Shared.Utils;

namespace fluXis.Game.Online.API.Requests.Scores;

public class ScoreSubmitRequest : APIRequest<APIScoreResponse>
{
    protected override string Path => "/scores/upload";
    protected override HttpMethod Method => HttpMethod.Post;

    private ScoreInfo score { get; }

    public ScoreSubmitRequest(ScoreInfo score)
    {
        this.score = score;
    }

    protected override void CreatePostData(FluXisJsonWebRequest<APIScoreResponse> request)
    {
        request.AddRaw(score.Serialize());
    }
}
