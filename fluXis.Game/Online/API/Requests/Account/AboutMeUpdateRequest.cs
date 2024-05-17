using System.Net.Http;
using osu.Framework.IO.Network;

namespace fluXis.Game.Online.API.Requests.Account;

public class AboutMeUpdateRequest : APIRequest<dynamic>
{
    protected override string Path => "/account/update/aboutme";
    protected override HttpMethod Method => HttpMethod.Post;

    private string about { get; }

    public AboutMeUpdateRequest(string about)
    {
        this.about = about;
    }

    protected override WebRequest CreateWebRequest(string url)
    {
        var req = base.CreateWebRequest(url);
        req.AddRaw(about);
        return req;
    }
}
