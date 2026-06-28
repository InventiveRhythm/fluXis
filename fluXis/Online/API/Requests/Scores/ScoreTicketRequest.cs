using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using fluXis.Mods;
using fluXis.Online.API.Payloads.Scores;
using Midori.Utils;
using osu.Framework.IO.Network;

namespace fluXis.Online.API.Requests.Scores;

public class ScoreTicketRequest : APIRequest<string>
{
    protected override string Path => "/scores/ticket";
    protected override HttpMethod Method => HttpMethod.Post;

    private string mapHash { get; }
    private string effectHash { get; }
    private string storyboardHash { get; }
    private List<IMod> mods { get; }

    public ScoreTicketRequest(string mapHash, string effectHash, string storyboardHash, List<IMod> mods)
    {
        this.mapHash = mapHash;
        this.effectHash = effectHash;
        this.storyboardHash = storyboardHash;
        this.mods = mods;
    }

    protected override WebRequest CreateWebRequest(string url)
    {
        var req = base.CreateWebRequest(url);
        req.AddRaw(new ScoreTicketPayload
        {
            MapHash = mapHash,
            EffectHash = effectHash,
            StoryboardHash = storyboardHash,
            Mods = mods.Select(x => x.Acronym).ToList()
        }.Serialize());
        return req;
    }
}
