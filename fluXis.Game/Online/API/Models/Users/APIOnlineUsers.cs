using System.Collections.Generic;
using Newtonsoft.Json;

namespace fluXis.Game.Online.API.Models.Users;

public class APIOnlineUsers
{
    [JsonProperty("count")]
    public int Count { get; init; }

    [JsonProperty("users")]
    public List<APIUserShort> Users { get; init; }
}
