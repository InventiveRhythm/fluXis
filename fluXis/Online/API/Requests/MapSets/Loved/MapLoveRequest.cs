using fluXis.Online.API.Models.Maps;

namespace fluXis.Online.API.Requests.MapSets.Loved;

public class MapLoveRequest : APIRequest<APIMapSetLoveState>
{
    protected override string Path => $"/mapset/{id}/love";

    private long id { get; }

    public MapLoveRequest(long id)
    {
        this.id = id;
    }
}
