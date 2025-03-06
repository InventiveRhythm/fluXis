using System.Net.Http;
using fluXis.Online.API.Payloads.Users;
using fluXis.Utils;
using Newtonsoft.Json.Linq;
using osu.Framework.IO.Network;

namespace fluXis.Online.API.Requests.Users;

public class UserConnectionCreateRequest : APIRequest<JToken>
{
    protected override string Path => $"/user/{id}/connections";
    protected override HttpMethod Method => HttpMethod.Post;

    private long id { get; }
    private string provider { get; }
    private string token { get; }

    public UserConnectionCreateRequest(long id, string provider, string token)
    {
        this.id = id;
        this.provider = provider;
        this.token = token;
    }

    protected override WebRequest CreateWebRequest(string url)
    {
        var req = base.CreateWebRequest(url);
        req.AddRaw(new UserConnectionCreatePayload
        {
            Provider = provider,
            Token = token
        }.Serialize());
        return req;
    }
}
