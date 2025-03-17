using System.Net.Http;
using fluXis.Utils;
using osu.Framework.IO.Network;

namespace fluXis.Online.API.Requests.Users;

public class ChangeUsernameRequest : APIRequest<object>
{
    protected override string Path => "/users/@me/username";
    protected override HttpMethod Method => HttpMethod.Patch;

    private string username { get; }

    public ChangeUsernameRequest(string username)
    {
        this.username = username;
    }

    protected override WebRequest CreateWebRequest(string url)
    {
        var req = base.CreateWebRequest(url);
        req.AddRaw(new { username }.Serialize());
        return req;
    }
}
