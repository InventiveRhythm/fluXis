using Newtonsoft.Json;

namespace fluXis.Shared.API.Payloads.Auth;

public class PasswordResetPayload
{
    [JsonProperty("token")]
    public string? Token { get; set; }

    [JsonProperty("password")]
    public string? Password { get; set; }
}
