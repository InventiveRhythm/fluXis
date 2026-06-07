using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace fluXis.Online.API.Payloads.Auth;

#nullable enable

public class PasswordResetPayload
{
    [JsonProperty("token")]
    [Required]
    public string Token { get; set; } = null!;

    [JsonProperty("password")]
    [Required]
    public string Password { get; set; } = null!;
}
