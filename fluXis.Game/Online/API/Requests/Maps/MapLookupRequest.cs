using fluXis.Shared.Components.Maps;
using osu.Framework.IO.Network;

namespace fluXis.Game.Online.API.Requests.Maps;

public class MapLookupRequest : APIRequest<APIMapLookup>
{
    protected override string Path => "/maps/lookup";

    private string hash { get; }

    public MapLookupRequest(string hash)
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
