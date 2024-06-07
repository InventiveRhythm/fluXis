using fluXis.Shared.Components.Users;

namespace fluXis.Game.Online.API.Requests.Users;

public class UserRequest : APIRequest<APIUser>
{
    protected override string Path => $"/user/{id}";

    private long id { get; }

    public UserRequest(long id)
    {
        this.id = id;
    }
}
