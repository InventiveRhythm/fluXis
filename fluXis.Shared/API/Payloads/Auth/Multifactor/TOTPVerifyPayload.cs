using Newtonsoft.Json;

namespace fluXis.Shared.API.Payloads.Auth.Multifactor;

public class TOTPVerifyPayload
{
    [JsonProperty("code")]
    public string? Code { get; set; }
}
