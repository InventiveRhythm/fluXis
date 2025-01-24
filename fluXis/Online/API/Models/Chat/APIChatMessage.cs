using fluXis.Online.API.Models.Users;
using Newtonsoft.Json;

namespace fluXis.Online.API.Models.Chat;

public class APIChatMessage
{
    [JsonProperty("id")]
    public string ID { get; init; } = null!;

    [JsonProperty("created")]
    public long CreatedAtUnix { get; init; }

    [JsonProperty("content")]
    public string Content { get; init; } = null!;

    [JsonProperty("channel")]
    public string Channel { get; init; } = null!;

    [JsonProperty("sender")]
    public APIUser Sender { get; init; } = null!;
}
