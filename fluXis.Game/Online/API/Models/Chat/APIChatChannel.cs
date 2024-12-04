using Newtonsoft.Json;

namespace fluXis.Game.Online.API.Models.Chat;

public class APIChatChannel
{
    [JsonProperty("name")]
    public string Name { get; init; } = null!;

    [JsonProperty("type")]
    public APIChannelType Type { get; init; }

    [JsonProperty("count")]
    public long UserCount { get; init; }
}

public enum APIChannelType
{
    Public = 0,
    Private = 1
}
