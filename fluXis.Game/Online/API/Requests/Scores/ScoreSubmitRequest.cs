using System.Net.Http;
using fluXis.Game.Online.API.Models.Scores;
using fluXis.Game.Scoring;
using Newtonsoft.Json;
using osu.Framework.IO.Network;

namespace fluXis.Game.Online.API.Requests.Scores;

public class ScoreSubmitRequest : APIRequest<APIScoreResponse>
{
    protected override string Path => "/scores/upload";
    protected override HttpMethod Method => HttpMethod.Post;

    private ScoreInfo score { get; }
    private float scrollspeed { get; }

    public ScoreSubmitRequest(ScoreInfo score, float scrollspeed)
    {
        this.score = score;
        this.scrollspeed = scrollspeed;
    }

    protected override void CreatePostData(JsonWebRequest<APIResponse<APIScoreResponse>> request)
    {
        request.AddRaw(JsonConvert.SerializeObject(new
        {
            hash = score.MapHash,
            mods = score.Mods,
            scrollSpeed = scrollspeed,
            maxCombo = score.MaxCombo,
            flawless = score.Flawless,
            perfect = score.Perfect,
            great = score.Great,
            alright = score.Alright,
            okay = score.Okay,
            miss = score.Miss
        }));
    }
}
