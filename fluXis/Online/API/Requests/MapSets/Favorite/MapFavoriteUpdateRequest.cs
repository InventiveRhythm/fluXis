using System.Net.Http;
using fluXis.Online.API.Models.Maps;
using fluXis.Utils;
using osu.Framework.IO.Network;

namespace fluXis.Online.API.Requests.MapSets.Favorite;

public class MapFavoriteUpdateRequest : APIRequest<APIMapSetFavoriteState>
{
    protected override string Path => $"/mapset/{id}/favorite";
    protected override HttpMethod Method => HttpMethod.Patch;

    private long id { get; }
    private bool value { get; }

    public MapFavoriteUpdateRequest(long id, bool value)
    {
        this.id = id;
        this.value = value;
    }

    protected override WebRequest CreateWebRequest(string url)
    {
        var req = base.CreateWebRequest(url);
        req.AddRaw(new APIMapSetFavoriteState { Favorite = value }.Serialize());
        return req;
    }
}
