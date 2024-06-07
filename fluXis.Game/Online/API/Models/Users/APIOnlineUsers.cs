using System.Collections.Generic;
using fluXis.Shared.Components.Users;
using Newtonsoft.Json;

namespace fluXis.Game.Online.API.Models.Users;

public class APIOnlineUsers
{
    [JsonProperty("count")]
    public int Count { get; init; }

    [JsonProperty("users")]
    public List<APIUser> Users { get; init; }
}
