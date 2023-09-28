using System;
using System.Linq;
using System.Net.Http;
using fluXis.Game.Configuration;
using fluXis.Game.Online.API;
using fluXis.Game.Online.API.Scores;
using fluXis.Game.Scoring;
using Newtonsoft.Json;

namespace fluXis.Game.Online.Scores;

public static class OnlineScores
{
    public static async void UploadScore(Fluxel.Fluxel fluxel, ScoreInfo score, Action<APIResponse<APIScoreResponse>> callback)
    {
        if (fluxel.Token == null)
        {
            callback(new APIResponse<APIScoreResponse>(401, "No token", null));
            return;
        }

        if (score.Mods.Any(m => m == "PA"))
        {
            callback(new APIResponse<APIScoreResponse>(400, "Score not submittable.", null));
            return;
        }

        var submitScore = new
        {
            hash = score.MapHash,
            mods = score.Mods,
            scrollSpeed = fluxel.Config.Get<float>(FluXisSetting.ScrollSpeed),
            maxCombo = score.MaxCombo,
            flawless = score.Flawless,
            perfect = score.Perfect,
            great = score.Great,
            alright = score.Alright,
            okay = score.Okay,
            miss = score.Miss
        };

        var req = fluxel.CreateAPIRequest("/scores/upload", HttpMethod.Post);
        req.AddRaw(JsonConvert.SerializeObject(submitScore));
        await req.PerformAsync();

        callback(JsonConvert.DeserializeObject<APIResponse<APIScoreResponse>>(req.GetResponseString()));
    }
}
