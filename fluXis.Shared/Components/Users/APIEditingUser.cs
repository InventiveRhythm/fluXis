using Newtonsoft.Json;

namespace fluXis.Shared.Components.Users;

public class APIEditingUser : APIUserShort
{
    [JsonProperty("email")]
    public string Email { get; set; } = null!;

    [JsonProperty("about")]
    public string? AboutMe { get; set; }

    [JsonProperty("pronouns")]
    public string? Pronouns { get; set; }

    [JsonProperty("social")]
    public IAPIUserSocials Socials { get; set; } = null!;
}

