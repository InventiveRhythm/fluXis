using Newtonsoft.Json;

namespace fluXis.Shared.API.Parameters.Auth.Multifactor;

public class TOTPEnableParameters
{
    [JsonProperty("pwd")]
    public string? Password { get; set; }

    [JsonProperty("key")]
    public string? SharedKey { get; set; }

    [JsonProperty("code")]
    public string? Code { get; set; }
}
