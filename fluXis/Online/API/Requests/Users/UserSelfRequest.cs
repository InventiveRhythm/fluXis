using fluXis.Online.API.Models.Users;

namespace fluXis.Online.API.Requests.Users;

public class UserSelfRequest : APIRequest<APIUser>
{
    protected override string Path => $"/users/@me";
}
