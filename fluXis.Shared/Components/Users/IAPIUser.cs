using fluXis.Shared.Components.Groups;
using Newtonsoft.Json;

namespace fluXis.Shared.Components.Users;

public interface IAPIUser : IAPIUserShort
{
    [JsonProperty("aboutme")]
    public string AboutMe { get; set; }

    [JsonProperty("pronouns")]
    public string Pronouns { get; set; }

    [JsonProperty("social")]
    public IAPIUserSocials Socials { get; init; }

    [JsonProperty("created")]
    public long CreatedAt { get; init; }

    [JsonProperty("lastlogin")]
    public long LastLogin { get; set; }

    [JsonProperty("is_online")]
    public bool IsOnline { get; init; }

    [JsonProperty("ovr")]
    public double OverallRating { get; set; }

    [JsonProperty("ptr")]
    public double PotentialRating { get; set; }

    [JsonProperty("max_combo")]
    public int MaxCombo { get; set; }

    [JsonProperty("ranked_score")]
    public int RankedScore { get; set; }

    [JsonProperty("ova")]
    public double OverallAccuracy { get; set; }

    [JsonProperty("rank")]
    public int GlobalRank { get; init; }

    [JsonProperty("country_rank")]
    public int CountryRank { get; init; }

    [JsonProperty("following")]
    public bool Following { get; set; }
}

// this is just to make implementing the interface easier
// should never actually be used as a field type
// use <see cref="APIUserShort"/> instead
public interface IAPIUserShort
{
    [JsonProperty("id")]
    long ID { get; init; }

    [JsonProperty("username")]
    string Username { get; set; }

    [JsonProperty("displayname")]
    string? DisplayName { get; set; }

    [JsonProperty("country")]
    string? CountryCode { get; set; }

    [JsonProperty("groups")]
    List<IAPIGroup> Groups { get; init; }
}
