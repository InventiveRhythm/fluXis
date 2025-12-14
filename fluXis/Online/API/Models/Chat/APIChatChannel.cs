using System.ComponentModel;
using fluXis.Online.API.Models.Users;
using Newtonsoft.Json;

namespace fluXis.Online.API.Models.Chat;

#nullable enable

public class APIChatChannel
{
    [JsonProperty("name")]
    public string Name { get; init; } = null!;

    [JsonProperty("type")]
    public APIChannelType Type { get; init; }

    [JsonProperty("count")]
    public long UserCount { get; init; }

    [JsonProperty("target-1")]
    public APIUser? Target1 { get; init; }

    [JsonProperty("target-2")]
    public APIUser? Target2 { get; init; }

    public APIUser? OtherUser(long self) => Target1?.ID == self ? Target2 : Target1;
}

public enum APIChannelType
{
    [Description("Public Channels")]
    Public = 0,

    [Description("Direct Messages")]
    Private = 1
}
