using fluXis.Game.Online.API.Users;
using Newtonsoft.Json;

namespace fluXis.Game.Online.Chat;

public class ChatMessage
{
    [JsonProperty("id")]
    public string Id { get; set; }

    [JsonProperty("created")]
    public long Timestamp { get; set; }

    [JsonProperty("content")]
    public string Content { get; set; }

    [JsonProperty("channel")]
    public string Channel { get; set; }

    [JsonProperty("sender")]
    public APIUserShort Sender { get; set; }
}
