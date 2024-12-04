using System;
using System.Collections.Generic;
using fluXis.Game.Online.API.Models.Maps;
using fluXis.Game.Utils;
using Newtonsoft.Json;

namespace fluXis.Game.Online.API.Models.Users;

public class APIUserMaps
{
    [JsonProperty("ranked")]
    public List<APIMapSet> Pure { get; set; } = null!;

    [JsonProperty("unranked")]
    public List<APIMapSet> Impure { get; set; } = null!;

    [JsonProperty("guest_diffs")]
    public List<APIMapSet> Guest { get; set; } = null!;

    public APIUserMaps(List<APIMapSet> pure, List<APIMapSet> impure, List<APIMapSet> guest)
    {
        Pure = pure;
        Impure = impure;
        Guest = guest;
    }

    [JsonConstructor]
    [Obsolete(JsonUtils.JSON_CONSTRUCTOR_ERROR, true)]
    public APIUserMaps()
    {
    }
}
