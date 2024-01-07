using fluXis.Game.Online.API.Models.Users;

namespace fluXis.Game.Online.API.Requests.Users;

public class UserMapsRequest : APIRequest<APIUserMaps>
{
    protected override string Path => $"/user/{id}/maps";

    private long id { get; }

    public UserMapsRequest(long id)
    {
        this.id = id;
    }
}
