using fluXis.Online.API.Models.Maps;

namespace fluXis.Online.API.Requests.MapSets;

public class MapSetBundledRequest : APIRequest<APIMapSet[]>
{
    protected override string Path => "/mapsets/bundled";
}
