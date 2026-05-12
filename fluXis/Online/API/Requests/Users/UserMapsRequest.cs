using fluXis.Online.API.Models.Users;

namespace fluXis.Online.API.Requests.Users;

public class UserMapsRequest : APIRequest<APIUserMaps>
{
    protected override string Path => $"/users/{id}/maps";

    private long id { get; }

    public UserMapsRequest(long id)
    {
        this.id = id;
    }
}
