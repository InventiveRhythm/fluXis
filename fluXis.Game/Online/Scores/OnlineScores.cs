using System;
using System.Linq;
using fluXis.Game.Configuration;
using fluXis.Game.Online.API;
using fluXis.Game.Online.API.Models.Scores;
using fluXis.Game.Online.API.Requests.Scores;
using fluXis.Game.Scoring;

namespace fluXis.Game.Online.Scores;

public static class OnlineScores
{
    public static async void UploadScore(Fluxel.Fluxel fluxel, ScoreInfo score, Action<APIResponse<APIScoreResponse>> callback)
    {
        if (fluxel.Token == null)
        {
            callback(new APIResponse<APIScoreResponse>(401, "Not logged in.", null));
            return;
        }

        if (score.Mods.Any(m => m == "PA"))
        {
            callback(new APIResponse<APIScoreResponse>(400, "Score not submittable.", null));
            return;
        }

        var req = new ScoreSubmitRequest(score, fluxel.Config.Get<float>(FluXisSetting.ScrollSpeed));
        await req.PerformAsync(fluxel);
        callback(req.Response);
    }
}
