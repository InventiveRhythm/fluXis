using System.Net.Http;

namespace fluXis.Online.API.Requests.Users;

public class UserFollowRequest : APIRequest<dynamic>
{
    protected override string Path => $"/users/{id}/follow";
    protected override HttpMethod Method => unfollow ? HttpMethod.Delete : HttpMethod.Put;

    private long id { get; }
    private bool unfollow { get; }

    public UserFollowRequest(long id, bool unfollow = false)
    {
        this.id = id;
        this.unfollow = unfollow;
    }
}
