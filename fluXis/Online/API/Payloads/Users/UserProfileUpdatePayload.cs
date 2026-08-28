using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace fluXis.Online.API.Payloads.Users;

#nullable enable

public class UserProfileUpdatePayload
{
    /// <summary>
    /// base64 encoded image
    /// </summary>
    [JsonProperty("avatar")]
    [Base64String]
    public string? Avatar { get; set; }

    /// <summary>
    /// base64 encoded image
    /// </summary>
    [JsonProperty("banner")]
    [Base64String]
    public string? Banner { get; set; }

    [JsonProperty("nick")]
    [RegularExpression("^.{2,24}$", ErrorMessage = "Display name must be between 2-24 characters.")]
    public string? DisplayName { get; set; }

    [JsonProperty("about")]
    [MaxLength(256, ErrorMessage = "AboutMe must be shorter than 256 characters.")]
    public string? AboutMe { get; set; }

    [JsonProperty("pronouns")]
    [MaxLength(16, ErrorMessage = "Pronouns must be shorter than 16 characters.")]
    public string? Pronouns { get; set; }
}
