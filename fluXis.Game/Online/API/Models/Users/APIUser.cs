using fluXis.Game.Online.API.Models.Clubs;
using fluXis.Game.Online.API.Models.Maps;
using fluXis.Game.Online.API.Models.Scores;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace fluXis.Game.Online.API.Models.Users;

public class APIUser : APIUserShort
{
    [JsonProperty("aboutme")]
    public string AboutMe { get; set; } = string.Empty;

    [CanBeNull]
    [JsonProperty("club")]
    public APIClubShort Club { get; set; } = new();

    [JsonProperty("social")]
    public APIUserSocials Socials { get; set; } = new();

    [JsonProperty("created")]
    public long Created { get; set; }

    [JsonProperty("lastlogin")]
    public long LastLogin { get; set; }

    [JsonProperty("is_online")]
    public bool Online { get; set; }

    [JsonProperty("ovr")]
    public double OverallRating { get; set; }

    [JsonProperty("ptr")]
    public double PotentialRating { get; set; }

    [JsonProperty("ova")]
    public float OverallAccuracy { get; set; }

    [JsonProperty("recent_scores")]
    public APIScore[] RecentScores { get; set; }

    [JsonProperty("best_scores")]
    public APIScore[] BestScores { get; set; }

    [JsonProperty("max_combo")]
    public int MaxCombo { get; set; }

    [JsonProperty("ranked_score")]
    public int RankedScore { get; set; }

    [JsonProperty("rank")]
    public int GlobalRank { get; set; }

    [JsonProperty("country_rank")]
    public int CountryRank { get; set; }

    [JsonProperty("ranked_maps")]
    public APIMapSet[] RankedMaps { get; set; }

    [JsonProperty("unranked_maps")]
    public APIMapSet[] UnrankedMaps { get; set; }

    [JsonProperty("guest_diffs")]
    public APIMapSet[] GuestDiffs { get; set; }

    public static APIUser DummyUser(int id, string username = "Player")
    {
        return new APIUser
        {
            ID = id,
            Username = username
        };
    }

    public static string GetRole(int role)
    {
        return role switch
        {
            1 => "Featured Artist",
            2 => "Purifier",
            3 => "Moderator",
            4 => "Admin",
            5 => "Bot",
            _ => "User"
        };
    }
}
