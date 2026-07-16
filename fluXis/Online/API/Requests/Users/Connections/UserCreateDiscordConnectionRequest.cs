using System.Net.Http;
using fluXis.Online.API.Models.Users.Connections;
using Midori.Utils;
using osu.Framework.IO.Network;

namespace fluXis.Online.API.Requests.Users.Connections;

public class UserCreateDiscordConnectionRequest : APIRequest<string>
{
    protected override string Path => $"/users/{uid}/connections/discord";
    protected override HttpMethod Method => HttpMethod.Put;

    private long uid { get; }
    private string code { get; }
    private string redirect { get; }
    private string verify { get; }

    public UserCreateDiscordConnectionRequest(long uid, string code, string redirect, string verify)
    {
        this.uid = uid;
        this.code = code;
        this.redirect = redirect;
        this.verify = verify;
    }

    protected override WebRequest CreateWebRequest(string url)
    {
        var req = base.CreateWebRequest(url);
        req.AddRaw(new DiscordConnectionPayload
        {
            Code = code,
            RedirectUri = redirect,
            CodeVerifier = verify
        }.Serialize());
        return req;
    }
}
