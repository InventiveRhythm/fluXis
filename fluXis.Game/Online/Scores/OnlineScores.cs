using System;
using System.Collections.Generic;
using System.Net.Http;
using fluXis.Game.Online.API;
using fluXis.Game.Scoring;
using Newtonsoft.Json;
using osu.Framework.IO.Network;

namespace fluXis.Game.Online.Scores;

public class OnlineScores
{
    public static async void UploadScore(Performance performance, Action<APIResponse<dynamic>> callback)
    {
        if (Fluxel.Fluxel.Token == null)
        {
            callback(new APIResponse<dynamic>(401, "No token", null));
            return;
        }

        var score = new APIScore
        {
            Score = performance.Score,
            Accuracy = performance.Accuracy,
            MaxCombo = performance.MaxCombo,
            Judgements = new Dictionary<string, int>
            {
                { "flawless", performance.Judgements.TryGetValue(Judgement.Flawless, out var flawless) ? flawless : 0 },
                { "perfect", performance.Judgements.TryGetValue(Judgement.Perfect, out var perfect) ? perfect : 0 },
                { "great", performance.Judgements.TryGetValue(Judgement.Great, out var great) ? great : 0 },
                { "alright", performance.Judgements.TryGetValue(Judgement.Alright, out var alright) ? alright : 0 },
                { "okay", performance.Judgements.TryGetValue(Judgement.Okay, out var okay) ? okay : 0 },
                { "miss", performance.Judgements.TryGetValue(Judgement.Miss, out var miss) ? miss : 0 }
            },
            HitStats = performance.HitStats,
            MapID = performance.MapID,
            MapHash = performance.MapHash,
            PlayerID = Fluxel.Fluxel.LoggedInUser?.ID ?? -1
        };

        var req = new WebRequest($"{APIConstants.APIUrl}/score");
        req.AddHeader("Authorization", Fluxel.Fluxel.Token);
        req.Method = HttpMethod.Post;
        req.AddRaw(JsonConvert.SerializeObject(score));
        await req.PerformAsync();
        callback(JsonConvert.DeserializeObject<APIResponse<dynamic>>(req.GetResponseString()));
    }
}
