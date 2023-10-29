using fluXis.Game.Online.API.Models.Maps;

namespace fluXis.Game.Online.API.Requests.Maps;

public class MapHashLookupRequest : APIRequest<APIMap>
{
    protected override string Path => $"/map/hash/{hash}";

    private string hash { get; }

    public MapHashLookupRequest(string hash)
    {
        this.hash = hash;
    }
}
