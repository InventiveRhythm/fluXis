using System;
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

        HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, APIConstants.API_URL + "/score");
        request.Headers.Add("Authorization", Fluxel.Fluxel.Token);
        request.Content = new StringContent(JsonConvert.SerializeObject(performance));
        var res = await Fluxel.Fluxel.Http.SendAsync(request);
        var response = await res.Content.ReadAsStringAsync();
        callback(JsonConvert.DeserializeObject<APIResponse<dynamic>>(response));
    }
}
