using fluXis.Shared.Utils;
using Newtonsoft.Json;

namespace fluXis.Shared.API.Payloads.Chat;

public class ChatMessagePayload
{
    [JsonProperty("content")]
    public string? Content { get; set; }

    public ChatMessagePayload(string content)
    {
        Content = content;
    }

    [JsonConstructor]
    [Obsolete(JsonUtils.JSON_CONSTRUCTOR_ERROR, true)]
    public ChatMessagePayload()
    {
    }
}
