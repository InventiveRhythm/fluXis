using System;
using System.Collections.Generic;
using System.Net.Http;
using fluXis.Game.Online.API;
using fluXis.Game.Scoring;
using Newtonsoft.Json;

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
                { "flawless", performance.Judgements.ContainsKey(Judgement.Flawless) ? performance.Judgements[Judgement.Flawless] : 0 },
                { "perfect", performance.Judgements.ContainsKey(Judgement.Perfect) ? performance.Judgements[Judgement.Perfect] : 0 },
                { "great", performance.Judgements.ContainsKey(Judgement.Great) ? performance.Judgements[Judgement.Great] : 0 },
                { "alright", performance.Judgements.ContainsKey(Judgement.Alright) ? performance.Judgements[Judgement.Alright] : 0 },
                { "okay", performance.Judgements.ContainsKey(Judgement.Okay) ? performance.Judgements[Judgement.Okay] : 0 },
                { "miss", performance.Judgements.ContainsKey(Judgement.Miss) ? performance.Judgements[Judgement.Miss] : 0 }
            },
            HitStats = performance.HitStats,
            MapID = performance.MapID,
            MapHash = performance.MapHash,
            PlayerID = Fluxel.Fluxel.GetLoggedInUser()?.ID ?? -1
        };

        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, APIConstants.APIUrl + "/score");
        request.Headers.Add("Authorization", Fluxel.Fluxel.Token);
        request.Content = new StringContent(JsonConvert.SerializeObject(score));
        var res = await Fluxel.Fluxel.Http.SendAsync(request);
        var response = await res.Content.ReadAsStringAsync();
        callback(JsonConvert.DeserializeObject<APIResponse<dynamic>>(response));
    }
}
