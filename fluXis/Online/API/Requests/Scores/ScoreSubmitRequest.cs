using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using fluXis.Mods;
using fluXis.Online.API.Payloads.Scores;
using fluXis.Online.API.Responses.Scores;
using fluXis.Replays;
using fluXis.Scoring;
using fluXis.Utils;
using osu.Framework.IO.Network;

namespace fluXis.Online.API.Requests.Scores;

public class ScoreSubmitRequest : APIRequest<ScoreSubmissionStats>
{
    protected override string Path => "/scores";
    protected override HttpMethod Method => HttpMethod.Post;

    private ScoreInfo score { get; }
    private List<IMod> mods { get; }
    private Replay replay { get; }
    private string mapHash { get; }
    private string effectHash { get; }
    private string storyboardHash { get; }

    public ScoreSubmitRequest(ScoreInfo score, List<IMod> mods, Replay replay, string hash, string eHash, string sHash)
    {
        if (score.Players is null)
            throw new InvalidOperationException();

        if (score.Players.Count == 0)
            throw new InvalidOperationException();

        if (score.Players[0].HitResults is null)
            throw new InvalidOperationException();

        this.score = score;
        this.mods = mods;
        this.replay = replay;
        mapHash = hash;
        effectHash = eHash;
        storyboardHash = sHash;
    }

    protected override WebRequest CreateWebRequest(string url)
    {
        var req = base.CreateWebRequest(url);

        var payload = new ScoreSubmissionPayload
        {
            MapHash = mapHash,
            EffectHash = effectHash,
            StoryboardHash = storyboardHash,
            Mods = mods.Select(x => x.Acronym).ToList(),
            Replay = replay,
            Scores = new List<ScoreSubmissionPayload.Player>()
        };

        foreach (var playerScore in score.Players)
        {
            var results = playerScore.HitResults.Select(x => new ScoreSubmissionPayload.Result
            {
                Difference = x.Difference,
                HoldEnd = x.HoldEnd
            });

            payload.Scores.Add(new ScoreSubmissionPayload.Player
            {
                UserID = playerScore.PlayerID,
                ScrollSpeed = playerScore.ScrollSpeed,
                Results = results.ToList()
            });
        }

        req.AddRaw(payload.Serialize());

        return req;
    }
}
