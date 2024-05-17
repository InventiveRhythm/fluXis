using System;
using fluXis.Game.Online.API;
using fluXis.Game.Online.API.Models.Scores;
using fluXis.Game.Online.API.Requests.Scores;
using fluXis.Game.Online.Fluxel;
using fluXis.Shared.Scoring;

namespace fluXis.Game.Online.Scores;

public static class OnlineScores
{
    public static async void UploadScore(FluxelClient fluxel, ScoreInfo score, Action<APIResponse<APIScoreResponse>> callback)
    {
        var req = new ScoreSubmitRequest(score);
        await fluxel.PerformRequestAsync(req);
        callback(req.Response);
    }
}
