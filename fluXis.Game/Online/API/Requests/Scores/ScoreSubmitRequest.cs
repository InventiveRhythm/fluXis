using System.Net.Http;
using fluXis.Shared.API.Responses.Scores;
using fluXis.Shared.Scoring;
using fluXis.Shared.Utils;
using osu.Framework.IO.Network;

namespace fluXis.Game.Online.API.Requests.Scores;

public class ScoreSubmitRequest : APIRequest<ScoreSubmissionStats>
{
    protected override string Path => "/scores";
    protected override HttpMethod Method => HttpMethod.Post;

    private ScoreInfo score { get; }

    public ScoreSubmitRequest(ScoreInfo score)
    {
        this.score = score;
    }

    protected override WebRequest CreateWebRequest(string url)
    {
        var req = base.CreateWebRequest(url);
        req.AddRaw(score.Serialize());
        return req;
    }
}
