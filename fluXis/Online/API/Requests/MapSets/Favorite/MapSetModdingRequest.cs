using System.Collections.Generic;
using fluXis.Online.API.Models.Maps.Modding;
using osu.Framework.IO.Network;

namespace fluXis.Online.API.Requests.MapSets.Favorite;

public class MapSetModdingRequest : APIRequest<List<APIModdingAction>>
{
    protected override string Path => $"/mapset/{id}/modding";
    private long id { get; }

    public MapSetModdingRequest(long id)
    {
        this.id = id;
    }

    protected override WebRequest CreateWebRequest(string url)
    {
        var req = base.CreateWebRequest(url);
        return req;
    }
}
