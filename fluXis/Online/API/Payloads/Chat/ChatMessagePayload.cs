using System;
using fluXis.Utils;
using Newtonsoft.Json;

namespace fluXis.Online.API.Payloads.Chat;

#nullable enable

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
