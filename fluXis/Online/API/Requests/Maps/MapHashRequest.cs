using fluXis.Online.API.Models.Maps;
using osu.Framework.IO.Network;

namespace fluXis.Online.API.Requests.Maps;

public class MapHashRequest : APIRequest<APIMapLookup>
{
    protected override string Path => "/search/maps/by-hash";

    private readonly string hash;

    public MapHashRequest(string hash)
    {
        this.hash = hash;
    }

    protected override WebRequest CreateWebRequest(string url)
    {
        var req = base.CreateWebRequest(url);
        req.AddParameter("hash", hash);
        return req;
    }
}
