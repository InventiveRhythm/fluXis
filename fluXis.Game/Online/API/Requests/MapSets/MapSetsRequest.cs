using System.Collections.Generic;
using fluXis.Game.Online.API.Models.Maps;

namespace fluXis.Game.Online.API.Requests.MapSets;

public class MapSetsRequest : APIRequest<List<APIMapSet>>
{
    protected override string Path => "/mapsets";
}
