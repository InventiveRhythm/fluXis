using fluXis.Online.API.Models.Maps;

namespace fluXis.Online.API.Requests.Maps;

public class MapRequest : APIRequest<APIMap>
{
    protected override string Path => $"/map/{id}";

    private long id { get; }

    public MapRequest(long id)
    {
        this.id = id;
    }
}
