using Newtonsoft.Json;

namespace fluXis.Game.Online.API.Payloads.Users;

#nullable enable

public class UserProfileUpdatePayload
{
    /// <summary>
    /// base64 encoded image
    /// </summary>
    [JsonProperty("avatar")]
    public string? Avatar { get; set; }

    /// <summary>
    /// base64 encoded image
    /// </summary>
    [JsonProperty("banner")]
    public string? Banner { get; set; }

    [JsonProperty("nick")]
    public string? DisplayName { get; set; }

    [JsonProperty("about")]
    public string? AboutMe { get; set; }

    [JsonProperty("twitter")]
    public string? Twitter { get; set; }

    [JsonProperty("discord")]
    public string? Discord { get; set; }

    [JsonProperty("twitch")]
    public string? Twitch { get; set; }

    [JsonProperty("youtube")]
    public string? YouTube { get; set; }

    [JsonProperty("pronouns")]
    public string? Pronouns { get; set; }
}
