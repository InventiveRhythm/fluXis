using System.Linq;
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

    public ScoreSubmitRequest(ScoreInfo score)
    {
        this.score = score;
    }

    public override void Perform(Fluxel.Fluxel fluxel)
    {
        if (fluxel.Token == null)
        {
            TriggerSuccess(new APIResponse<APIScoreResponse>(401, "Not logged in.", null));
            return;
        }

        if (score.Mods.Any(m => m == "PA"))
        {
            TriggerSuccess(new APIResponse<APIScoreResponse>(400, "Score not submittable.", null));
            return;
        }

        base.Perform(fluxel);
    }

    protected override void CreatePostData(JsonWebRequest<APIResponse<APIScoreResponse>> request)
    {
        request.AddRaw(JsonConvert.SerializeObject(new
        {
            hash = score.MapHash,
            mods = score.Mods,
            scrollSpeed = score.ScrollSpeed,
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
