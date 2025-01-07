using System.Collections.Generic;
using fluXis.Online.API.Models.Maps;
using osu.Framework.IO.Network;

namespace fluXis.Online.API.Requests.MapSets;

public class MapSetsRequest : APIRequest<List<APIMapSet>>
{
    protected override string Path => "/mapsets";

    private long offset { get; }
    private long limit { get; }
    private string query { get; }

    public MapSetsRequest(long offset, long limit = 50, string query = "")
    {
        this.offset = offset;
        this.limit = limit;
        this.query = query;
    }

    protected override WebRequest CreateWebRequest(string url)
    {
        var req = base.CreateWebRequest(url);
        req.AddParameter("limit", $"{limit}");

        if (offset > 0)
            req.AddParameter("offset", $"{offset}");
        if (!string.IsNullOrWhiteSpace(query))
            req.AddParameter("q", $"{query}");

        return req;
    }
}
