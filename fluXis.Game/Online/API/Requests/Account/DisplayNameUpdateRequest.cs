using System.Net.Http;
using osu.Framework.IO.Network;

namespace fluXis.Game.Online.API.Requests.Account;

public class DisplayNameUpdateRequest : APIRequest<dynamic>
{
    protected override string Path => "/account/update/displayname";
    protected override HttpMethod Method => HttpMethod.Post;

    private string displayName { get; }

    public DisplayNameUpdateRequest(string displayName)
    {
        this.displayName = displayName;
    }

    protected override WebRequest CreateWebRequest(string url)
    {
        var req = base.CreateWebRequest(url);
        req.AddRaw(displayName);
        return req;
    }
}
