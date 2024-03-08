using fluXis.Shared.Components.Users;
using Newtonsoft.Json;

namespace fluXis.Game.Online.API.Models.Users;

public class APIEditingUser : APIUserShort
{
    [JsonProperty("email")]
    public string Email { get; set; }

    [JsonProperty("about")]
    public string AboutMe { get; set; }

    [JsonProperty("social")]
    public APIUser.APIUserSocials Socials { get; set; }
}
