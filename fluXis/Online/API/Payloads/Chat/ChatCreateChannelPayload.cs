using Newtonsoft.Json;

namespace fluXis.Online.API.Payloads.Chat;

#nullable enable

public class ChatCreateChannelPayload
{
    [JsonProperty("target")]
    public long? TargetID { get; set; }
}
