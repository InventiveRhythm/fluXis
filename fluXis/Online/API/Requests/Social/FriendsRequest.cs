using fluXis.Online.API.Models.Social;

namespace fluXis.Online.API.Requests.Social;

public class FriendsRequest : APIRequest<APIFriends>
{
    protected override string Path => "/social/friends";
}
