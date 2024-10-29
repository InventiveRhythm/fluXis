using fluXis.Shared.Components.Maps;
using fluXis.Shared.Components.Users;
using Newtonsoft.Json;

namespace fluXis.Shared.Components.Collections;

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
