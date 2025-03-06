using Newtonsoft.Json;

namespace fluXis.Online.API.Payloads.Users;

#nullable enable

public class UserConnectionCreatePayload
{
    [JsonProperty("provider")]
    public string? Provider { get; set; }

    [JsonProperty("token")]
    public string? Token { get; set; }
}
