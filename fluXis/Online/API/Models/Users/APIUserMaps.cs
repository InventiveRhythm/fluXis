using System;
using fluXis.Online.API.Models.Maps;
using Midori.Utils;
using Newtonsoft.Json;

namespace fluXis.Online.API.Models.Users;

public class APIUserMaps
{
    [JsonProperty("ranked")]
    public APIMapSet[] Pure { get; set; } = null!;

    [JsonProperty("unranked")]
    public APIMapSet[] Impure { get; set; } = null!;

    [JsonProperty("guest_diffs")]
    public APIMapSet[] Guest { get; set; } = null!;

    [JsonProperty("limit_uploaded")]
    public long? LimitUploadedCount { get; set; }

    [JsonProperty("limit_max")]
    public long? LimitMaximumCount { get; set; }

    public APIUserMaps(APIMapSet[] pure, APIMapSet[] impure, APIMapSet[] guest)
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
