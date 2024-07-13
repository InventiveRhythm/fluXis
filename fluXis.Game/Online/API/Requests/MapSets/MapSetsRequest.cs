using System.Collections.Generic;
using fluXis.Shared.Components.Maps;
using osu.Framework.IO.Network;

namespace fluXis.Game.Online.API.Requests.MapSets;

public class MapSetsRequest : APIRequest<List<APIMapSet>>
{
    protected override string Path => "/mapsets";

    private long offset { get; }

    public MapSetsRequest(long offset)
    {
        this.offset = offset;
    }

    protected override WebRequest CreateWebRequest(string url)
    {
        var req = base.CreateWebRequest(url);

        if (offset > 0)
            req.AddParameter("offset", $"{offset}");

        return req;
    }
}
