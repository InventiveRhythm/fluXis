using System.Collections.Generic;
using fluXis.Shared.Components.Users;

namespace fluXis.Game.Online.API.Requests.Users;

public class UserFollowersRequest : APIRequest<List<APIUserShort>>
{
    protected override string Path => $"/user/{id}/followers";

    private long id { get; }

    public UserFollowersRequest(long id)
    {
        this.id = id;
    }
}
