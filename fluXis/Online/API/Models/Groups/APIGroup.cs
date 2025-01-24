using System;
using System.Collections.Generic;
using fluXis.Online.API.Models.Users;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace fluXis.Online.API.Models.Groups;

public class APIGroup
{
    [JsonProperty("id")]
    public string ID { get; init; } = "";

    [JsonProperty("name")]
    public string Name { get; set; } = "";

    [JsonProperty("tag")]
    public string Tag { get; set; } = "";

    [JsonProperty("color")]
    public string Color { get; set; } = "#ffffff";

    [CanBeNull]
    [JsonProperty("members")]
    public IEnumerable<APIUser> Members { get; set; }

    public static APIGroup CreateDummy(string id) => new()
    {
        ID = id,
        Color = "#ffffff",
        Name = id,
        Tag = id,
        Members = Array.Empty<APIUser>()
    };
}
