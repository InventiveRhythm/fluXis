using System.Collections.Generic;
using fluXis.Online.API.Models.Maps;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.IO.Network;

namespace fluXis.Online.API.Requests.MapSets;

public class MapSetsRequest : APIRequest<List<APIMapSet>>
{
    protected override string Path => "/search/mapsets";

    private long offset { get; }
    private Dictionary<string, string> query { get; }

    public MapSetsRequest(long offset, Dictionary<string, string> query)
    {
        this.offset = offset;
        this.query = query;
    }

    protected override WebRequest CreateWebRequest(string url)
    {
        var req = base.CreateWebRequest(url);
        query.ForEach(x => req.AddParameter(x.Key, x.Value));
        if (offset > 0) req.AddParameter("offset", $"{offset}");
        return req;
    }
}
