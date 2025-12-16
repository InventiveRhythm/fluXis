using System;
using fluXis.Utils;
using Newtonsoft.Json;

namespace fluXis.Online.API.Payloads.Chat;

#nullable enable

public class ChatCreateChannelPayload
{
    [JsonProperty("target")]
    public long? TargetID { get; set; }

    public ChatCreateChannelPayload(long target)
    {
        TargetID = target;
    }

    [JsonConstructor]
    [Obsolete(JsonUtils.JSON_CONSTRUCTOR_ERROR, true)]
    public ChatCreateChannelPayload()
    {
    }
}
