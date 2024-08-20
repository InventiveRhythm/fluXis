using System.Net.Http;
using fluXis.Shared.API.Payloads.Auth.Multifactor;
using fluXis.Shared.API.Responses.Auth.Multifactor;
using fluXis.Shared.Utils;
using osu.Framework.IO.Network;

namespace fluXis.Game.Online.API.Requests.Auth.Multifactor;

public class TOTPVerifyRequest : APIRequest<TOTPVerifyResponse>
{
    protected override string Path => "/auth/mfa/totp";
    protected override HttpMethod Method => HttpMethod.Post;

    private string code { get; }

    public TOTPVerifyRequest(string code)
    {
        this.code = code;
    }

    protected override WebRequest CreateWebRequest(string url)
    {
        var req = base.CreateWebRequest(url);
        req.AddRaw(new TOTPVerifyPayload { Code = code }.Serialize());
        return req;
    }
}
