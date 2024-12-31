using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Online.API.Models.Users;
using fluXis.Utils;
using Newtonsoft.Json;

namespace fluXis.Online.API.Responses.Users;

public class OnlineUsers
{
    [JsonProperty("count")]
    public int Count { get; init; }

    [JsonProperty("users")]
    public List<APIUser> Users { get; init; } = new();

    public OnlineUsers(int count, IEnumerable<APIUser> users)
    {
        Count = count;
        Users = users.ToList();
    }

    [JsonConstructor]
    [Obsolete(JsonUtils.JSON_CONSTRUCTOR_ERROR, true)]
    public OnlineUsers()
    {
    }
}
