using System.Net.Http;

namespace fluXis.Online.API.Requests.Users.Connections;

public class UserGetDiscordConnectionRequest : APIRequest<string>
{
    protected override string Path => $"/users/{uid}/connections/discord";
    protected override HttpMethod Method => HttpMethod.Get;

    private long uid { get; }

    public UserGetDiscordConnectionRequest(long uid)
    {
        this.uid = uid;
    }
}
