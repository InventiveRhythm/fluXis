using Newtonsoft.Json;

namespace fluXis.Game.Online.API.Models.Chat;

public class APIChatChannel
{
    [JsonProperty("name")]
    public string Name { get; init; } = null!;

    [JsonProperty("type")]
    public ChannelType Type { get; init; }

    [JsonProperty("count")]
    public long UserCount { get; init; }
}

public enum ChannelType
{
    Public = 0,
    Private = 1
}
