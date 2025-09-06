using System;
using fluXis.Online.API.Models.Maps;
using Newtonsoft.Json;

namespace fluXis.Online.Collections;

#nullable enable

public class CollectionItem
{
    [JsonProperty("type")]
    public CollectionItemType Type { get; set; }

    [JsonProperty("map")]
    public APIMap? Map { get; set; }

    [JsonProperty("local-id")]
    public Guid? LocalID { get; set; }
}

public enum CollectionItemType
{
    Online,
    Local
}
