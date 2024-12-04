using System.Collections.Generic;
using fluXis.Game.Online.API.Models.Users;
using Newtonsoft.Json;

namespace fluXis.Game.Online.API.Models.Groups;

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
