using System.Collections.Generic;
using fluXis.Online.API.Models.Users;

namespace fluXis.Online.API.Requests.Users;

public class UserFollowersRequest : APIRequest<List<APIUser>>
{
    protected override string Path => $"/user/{id}/followers";

    private long id { get; }

    public UserFollowersRequest(long id)
    {
        this.id = id;
    }
}
