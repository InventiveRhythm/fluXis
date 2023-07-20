using System;
using System.Linq;
using System.Net.Http;
using fluXis.Game.Configuration;
using fluXis.Game.Mods;
using fluXis.Game.Online.API;
using fluXis.Game.Online.API.Scores;
using fluXis.Game.Scoring;
using Newtonsoft.Json;
using osu.Framework.IO.Network;

namespace fluXis.Game.Online.Scores;

public static class OnlineScores
{
    public static async void UploadScore(Fluxel.Fluxel fluxel, Performance performance, Action<APIResponse<APIScoreResponse>> callback)
    {
        if (fluxel.Token == null)
        {
            callback(new APIResponse<APIScoreResponse>(401, "No token", null));
            return;
        }

        if (performance.Mods.Any(m => m is PausedMod))
        {
            callback(new APIResponse<APIScoreResponse>(400, "Score not submittable.", null));
            return;
        }

        var score = new
        {
            hash = performance.MapHash,
            mods = string.Join(",", performance.Mods.Select(m => m.Acronym)),
            scrollSpeed = fluxel.Config.Get<float>(FluXisSetting.ScrollSpeed),
            maxCombo = performance.MaxCombo,
            flawless = performance.GetJudgementCount(Judgement.Flawless),
            perfect = performance.GetJudgementCount(Judgement.Perfect),
            great = performance.GetJudgementCount(Judgement.Great),
            alright = performance.GetJudgementCount(Judgement.Alright),
            okay = performance.GetJudgementCount(Judgement.Okay),
            miss = performance.GetJudgementCount(Judgement.Miss)
        };

        var req = new WebRequest($"{fluxel.Endpoint.APIUrl}/scores/upload");
        req.AddHeader("Authorization", fluxel.Token);
        req.AllowInsecureRequests = true;
        req.Method = HttpMethod.Post;
        req.AddRaw(JsonConvert.SerializeObject(score));
        await req.PerformAsync();

        callback(JsonConvert.DeserializeObject<APIResponse<APIScoreResponse>>(req.GetResponseString()));
    }
}
