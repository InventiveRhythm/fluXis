using fluXis.Shared.API.Responses.Users;

namespace fluXis.Game.Online.API.Requests.Users;

public class OnlineUsersRequest : APIRequest<OnlineUsers>
{
    protected override string Path => "/users/online";
}
