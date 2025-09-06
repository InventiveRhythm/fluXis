using System.Collections.Generic;
using Newtonsoft.Json;

namespace fluXis.Online.Collections;

public class Collection
{
    [JsonProperty("name")]
    public string Name { get; init; }

    [JsonProperty("type")]
    public CollectionType Type { get; init; }

    [JsonProperty("items")]
    public List<CollectionItem> Items { get; init; } = new();
}

public enum CollectionType
{
    Loved,
    Owned,
    Subscribed
}
