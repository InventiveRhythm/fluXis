using System.Collections.Generic;
using fluXis.Game.Online.API.Models.Maps;
using Newtonsoft.Json;

namespace fluXis.Game.Online.API.Models.Users;

public class APIUserMaps
{
    [JsonProperty("ranked")]
    public List<APIMapSet> Pure { get; set; }

    [JsonProperty("unranked")]
    public List<APIMapSet> Impure { get; set; }

    [JsonProperty("guest_diffs")]
    public List<APIMapSet> Guest { get; set; }
}
