using System;
using fluXis.Online.API.Models.Users;
using Midori.Utils;
using Newtonsoft.Json;

namespace fluXis.Online.API.Responses.Users;

public class OnlineUsers
{
    [JsonProperty("count")]
    public int Count { get; init; }

    [JsonProperty("users")]
    public APIUser[] Users { get; init; } = [];

    public OnlineUsers(int count, APIUser[] users)
    {
        Count = count;
        Users = users;
    }

    [JsonConstructor]
    [Obsolete(JsonUtils.JSON_CONSTRUCTOR_ERROR, true)]
    public OnlineUsers()
    {
    }
}
