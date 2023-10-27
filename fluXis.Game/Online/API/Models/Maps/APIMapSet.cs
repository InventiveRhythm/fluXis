using System.Collections.Generic;
using fluXis.Game.Online.API.Models.Users;
using Newtonsoft.Json;

namespace fluXis.Game.Online.API.Models.Maps;

public class APIMapSet
{
    [JsonProperty("id")]
    public int Id { get; init; }

    [JsonProperty("creator")]
    public APIUserShort Creator { get; init; }

    [JsonProperty("artist")]
    public string Artist { get; init; } = "";

    [JsonProperty("title")]
    public string Title { get; init; } = "";

    [JsonProperty("status")]
    public int Status { get; init; }

    [JsonProperty("maps")]
    public List<APIMap> Maps { get; init; }

    [JsonProperty("submitted")]
    public long Submitted { get; init; }

    [JsonProperty("last_updated")]
    public long LastUpdated { get; init; }

    [JsonProperty("tags")]
    public string[] Tags { get; init; }

    [JsonProperty("source")]
    public string Source { get; init; } = "";
}
