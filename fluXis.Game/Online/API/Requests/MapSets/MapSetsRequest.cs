using System.Collections.Generic;
using fluXis.Shared.Components.Maps;

namespace fluXis.Game.Online.API.Requests.MapSets;

public class MapSetsRequest : APIRequest<List<APIMapSet>>
{
    protected override string Path => "/mapsets";
}
