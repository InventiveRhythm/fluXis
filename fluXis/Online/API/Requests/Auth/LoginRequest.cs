using System.Net.Http;
using fluXis.Online.API.Payloads.Auth;
using fluXis.Online.API.Responses.Auth;
using Midori.Utils;
using osu.Framework.IO.Network;

namespace fluXis.Online.API.Requests.Auth;

public class LoginRequest : APIRequest<LoginResponse>
{
    protected override string Path => "/auth/login";
    protected override HttpMethod Method => HttpMethod.Post;

    private LoginPayload payload { get; }

    public LoginRequest(LoginPayload payload)
    {
        this.payload = payload;
    }

    protected override WebRequest CreateWebRequest(string url)
    {
        var req = base.CreateWebRequest(url);
        var json = payload.Serialize();
        req.AddRaw(json);
        return req;
    }
}
