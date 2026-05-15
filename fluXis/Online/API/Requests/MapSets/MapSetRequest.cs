using fluXis.Online.API.Models.Maps;

namespace fluXis.Online.API.Requests.MapSets;

public class MapSetRequest : APIRequest<APIMapSet>
{
    protected override string Path => $"/mapsets/{id}";

    private long id { get; }

    public MapSetRequest(long id)
    {
        this.id = id;
    }
}
