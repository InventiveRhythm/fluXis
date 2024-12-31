using System.Net.Http;
using fluXis.Online.API.Payloads.Auth.Multifactor;
using fluXis.Online.API.Responses.Auth.Multifactor;
using fluXis.Utils;
using osu.Framework.IO.Network;

namespace fluXis.Online.API.Requests.Auth.Multifactor;

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
