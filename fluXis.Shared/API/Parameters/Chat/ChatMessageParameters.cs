using fluXis.Shared.Utils;
using Newtonsoft.Json;

namespace fluXis.Shared.API.Parameters.Chat;

public class ChatMessageParameters
{
    [JsonProperty("content")]
    public string? Content { get; set; }

    public ChatMessageParameters(string content)
    {
        Content = content;
    }

    [JsonConstructor]
    [Obsolete(JsonUtils.JSON_CONSTRUCTOR_ERROR, true)]
    public ChatMessageParameters()
    {
    }
}
