using fluXis.Online.API.Models.Users;

namespace fluXis.Online.API.Requests.Users;

public class UserRequest : APIRequest<APIUser>
{
    protected override string Path => $"/users/{id}";

    private long id { get; }

    public UserRequest(long id)
    {
        this.id = id;
    }
}
