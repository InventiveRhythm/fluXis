using fluXis.Online.API.Responses.Users;

namespace fluXis.Online.API.Requests.Users;

public class OnlineUsersRequest : APIRequest<OnlineUsers>
{
    protected override string Path => "/users/online";
}
