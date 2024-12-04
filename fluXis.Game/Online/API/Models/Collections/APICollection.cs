using System.Collections.Generic;
using fluXis.Game.Online.API.Models.Maps;
using fluXis.Game.Online.API.Models.Users;
using Newtonsoft.Json;

namespace fluXis.Game.Online.API.Models.Collections;

public class APICollection
{
    [JsonProperty("id")]
    public string ID { get; set; } = null!;

    [JsonProperty("owner")]
    public APIUser Owner { get; set; } = null!;

    [JsonProperty("name")]
    public string Name { get; set; } = null!;

    [JsonProperty("description")]
    public string Description { get; set; } = null!;

    [JsonProperty("created")]
    public long CreatedAt { get; set; }

    [JsonProperty("updated")]
    public long LastUpdated { get; set; }

    [JsonProperty("maps")]
    public List<APIMap> MapIDs { get; set; } = null!;
}
