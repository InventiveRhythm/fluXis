using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace fluXis.Online.API.Models.Users.Connections;

public class DiscordConnectionPayload
{
    [JsonProperty("code")]
    [Required]
    public string Code { get; set; }

    [JsonProperty("redirect")]
    [Required]
    public string RedirectUri { get; set; }

    [JsonProperty("verify")]
    [Required]
    public string CodeVerifier { get; set; }
}
