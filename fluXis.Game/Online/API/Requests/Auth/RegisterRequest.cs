using System.Net.Http;
using fluXis.Shared.API.Parameters.Auth;
using fluXis.Shared.API.Responses.Auth;
using fluXis.Shared.Utils;
using osu.Framework.IO.Network;

namespace fluXis.Game.Online.API.Requests.Auth;

public class RegisterRequest : APIRequest<RegisterResponse>
{
    protected override string Path => "/auth/register";
    protected override HttpMethod Method => HttpMethod.Post;

    private string username { get; }
    private string password { get; }
    private string email { get; }

    public RegisterRequest(string username, string password, string email)
    {
        this.username = username;
        this.password = password;
        this.email = email;
    }

    protected override WebRequest CreateWebRequest(string url)
    {
        var req = base.CreateWebRequest(url);
        var json = new RegisterParameters(username, password, email);
        req.AddRaw(json.Serialize());
        return req;
    }
}
