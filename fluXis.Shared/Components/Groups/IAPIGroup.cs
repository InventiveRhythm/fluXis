using fluXis.Shared.Components.Users;
using Newtonsoft.Json;

namespace fluXis.Shared.Components.Groups;

public interface IAPIGroup
{
    [JsonProperty("id")]
    string ID { get; init; }

    [JsonProperty("name")]
    string Name { get; set; }

    [JsonProperty("tag")]
    string Tag { get; set; }

    [JsonProperty("color")]
    string Color { get; set; }

    [JsonProperty("members")]
    IEnumerable<APIUser> Members { get; }
}
