using fluXis.Online.API.Models.Users;
using Newtonsoft.Json;

namespace fluXis.Online.API.Models.Chat;

public interface IChatMessage
{
    [JsonProperty("id")]
    public string ID { get; }

    [JsonProperty("created")]
    public long CreatedAtUnix { get; }

    [JsonProperty("content")]
    public string Content { get; }

    [JsonProperty("channel")]
    public string Channel { get; }

    [JsonProperty("sender")]
    public APIUser Sender { get; }
}
