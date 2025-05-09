using System.Net.Http;
using osu.Framework.IO.Network;

namespace fluXis.Online.API.Requests.Invites;

public class AcceptInviteRequest : APIRequest<dynamic>
{
    protected override string Path => $"/invites/{code}";
    protected override HttpMethod Method => HttpMethod.Post;

    private string code { get; }

    public AcceptInviteRequest(string code)
    {
        this.code = code;
    }

    protected override WebRequest CreateWebRequest(string url)
    {
        var req = base.CreateWebRequest(url);
        req.AddRaw("yuh"); // nginx doesn't like an empty post request
        return req;
    }
}
