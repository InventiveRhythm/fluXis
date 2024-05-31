using Newtonsoft.Json;

namespace fluXis.Shared.API.Parameters.Users;

public class UserProfileUpdateParameters
{
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
