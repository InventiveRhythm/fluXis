using fluXis.Shared.Components.Clubs;
using fluXis.Shared.Components.Groups;
using fluXis.Shared.Components.Users.Equipment;
using Newtonsoft.Json;

namespace fluXis.Shared.Components.Users;

public class APIUser
{
    [JsonProperty("id")]
    public long ID { get; init; }

    [JsonProperty("username")]
    public string Username { get; set; } = null!;

    [JsonProperty("displayname")]
    public string? DisplayName { get; set; }

    [JsonProperty("aboutme")]
    public string? AboutMe { get; set; }

    [JsonProperty("pronouns")]
    public string? Pronouns { get; set; }

    [JsonProperty("paint")]
    public APINamePaint? NamePaint { get; set; }

    [JsonProperty("country")]
    public string? CountryCode { get; set; }

    [JsonProperty("groups")]
    public List<IAPIGroup> Groups { get; init; } = new();

    [JsonProperty("club")]
    public APIClub? Club { get; set; }

    [JsonProperty("online")]
    public bool IsOnline { get; init; }

    #region Optional

    [JsonProperty("created")]
    public long? CreatedAt { get; set; }

    [JsonProperty("lastlogin")]
    public long? LastLogin { get; set; }

    [JsonProperty("socials")]
    public APIUserSocials? Socials { get; set; }

    [JsonProperty("stats")]
    public APIUserStatistics? Statistics { get; set; }

    [JsonProperty("following")]
    public bool? Following { get; set; }

    [JsonProperty("email")]
    public string? Email { get; set; }

    #endregion

    #region Misc

    [JsonIgnore]
    public string PreferredName => string.IsNullOrWhiteSpace(DisplayName) ? Username : DisplayName;

    [JsonIgnore]
    public string NameWithApostrophe
    {
        get
        {
            var name = PreferredName;
            if (name.EndsWith('s') || name.EndsWith('z'))
                return name + "'";

            return name + "'s";
        }
    }

    public static APIUser CreateUnknown(long id) => new() { ID = id, Username = $"Unknown ({id})" };

    public static APIUser Dummy => new() { ID = -1, Username = "Dummy Player" };
    public static APIUser Default => new() { ID = -1, Username = "Player" };
    public static APIUser AutoPlay => new() { ID = 0, Username = "AutoPlay" };

    #endregion
}
