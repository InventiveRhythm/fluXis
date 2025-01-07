using Newtonsoft.Json;

namespace fluXis.Online.API.Payloads.Auth.Multifactor;

#nullable enable

public class TOTPVerifyPayload
{
    [JsonProperty("code")]
    public string? Code { get; set; }
}
