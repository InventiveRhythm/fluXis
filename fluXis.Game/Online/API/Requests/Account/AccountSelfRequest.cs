using fluXis.Shared.Components.Users;

namespace fluXis.Game.Online.API.Requests.Account;

public class AccountSelfRequest : APIRequest<APIEditingUser>
{
    protected override string Path => "/account";
}
