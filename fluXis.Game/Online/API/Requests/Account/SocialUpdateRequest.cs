using System.Collections.Generic;
using System.Net.Http;
using fluXis.Shared.Utils;
using osu.Framework.IO.Network;

namespace fluXis.Game.Online.API.Requests.Account;

public class SocialUpdateRequest : APIRequest<dynamic>
{
    protected override string Path => "/account/update/socials";
    protected override HttpMethod Method => HttpMethod.Post;

    private string twitter { get; }
    private string youtube { get; }
    private string twitch { get; }
    private string discord { get; }

    public SocialUpdateRequest(string twitter, string youtube, string twitch, string discord)
    {
        this.twitter = twitter;
        this.youtube = youtube;
        this.twitch = twitch;
        this.discord = discord;
    }

    protected override WebRequest CreateWebRequest(string url)
    {
        var req = base.CreateWebRequest(url);

        req.AddRaw(new Dictionary<string, string>
        {
            { "twitter", twitter },
            { "youtube", youtube },
            { "twitch", twitch },
            { "discord", discord }
        }.Serialize());

        return req;
    }
}
