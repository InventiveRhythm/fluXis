using System.Net.Http;
using fluXis.Game.Online.API.Models.Scores;
using fluXis.Shared.Scoring;
using fluXis.Shared.Utils;
using osu.Framework.IO.Network;

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

    protected override WebRequest CreateWebRequest(string url)
    {
        var req = base.CreateWebRequest(url);
        req.AddRaw(score.Serialize());
        return req;
    }
}
