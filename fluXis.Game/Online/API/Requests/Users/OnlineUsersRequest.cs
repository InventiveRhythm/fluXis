using fluXis.Game.Online.API.Models.Users;

namespace fluXis.Game.Online.API.Requests.Users;

public class OnlineUsersRequest : APIRequest<APIOnlineUsers>
{
    protected override string Path => "/users/online";
}
