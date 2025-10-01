using System;
using fluXis.Online.API.Models.Maps;
using Midori.Utils;
using Newtonsoft.Json;

namespace fluXis.Online.Collections;

#nullable enable

public class CollectionItem
{
    /// <summary>
    /// A 5-character unique ID in the current collection.
    /// </summary>
    [JsonProperty("id")]
    public string ID { get; init; } = RandomizeUtils.GenerateRandomString(5);

    [JsonProperty("type")]
    public CollectionItemType Type { get; set; }

    /// <summary>
    /// Non-null when <see cref="CollectionItem.Type"/> is <see cref="CollectionItemType.Online"/>.
    /// </summary>
    [JsonProperty("map")]
    public APIMap? Map { get; set; }

    /// <summary>
    /// Non-null when <see cref="CollectionItem.Type"/> is <see cref="CollectionItemType.Local"/>.
    /// </summary>
    [JsonProperty("local-id")]
    public Guid? LocalID { get; set; }

    /// <summary>
    /// Non-null when <see cref="CollectionItem.Type"/> is <see cref="CollectionItemType.Local"/>.
    /// </summary>
    [JsonProperty("local-title")]
    public string? LocalTitle { get; set; }

    /// <summary>
    /// Non-null when <see cref="CollectionItem.Type"/> is <see cref="CollectionItemType.Local"/>.
    /// </summary>
    [JsonProperty("local-artist")]
    public string? LocalArtist { get; set; }
}

public enum CollectionItemType
{
    Online,
    Local
}
