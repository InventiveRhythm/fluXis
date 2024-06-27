using Newtonsoft.Json;

namespace fluXis.Shared.API.Parameters.Auth.Multifactor;

public class TOTPVerifyParameters
{
    [JsonProperty("code")]
    public string? Code { get; set; }
}
