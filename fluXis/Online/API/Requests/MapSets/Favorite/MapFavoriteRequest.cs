using fluXis.Online.API.Models.Maps;

namespace fluXis.Online.API.Requests.MapSets.Favorite;

public class MapFavoriteRequest : APIRequest<APIMapSetFavoriteState>
{
    protected override string Path => $"/mapset/{id}/favorite";

    private long id { get; }

    public MapFavoriteRequest(long id)
    {
        this.id = id;
    }
}
