using fluXis.Shared.Components.Users;
using Newtonsoft.Json;

namespace fluXis.Shared.Components.Clubs;

public class APIClubInvite
{
    /// <summary>
    /// The unique invite code.
    /// </summary>
    [JsonProperty("code")]
    public string InviteCode { get; set; } = "";

    /// <summary>
    /// The club the invite goes to.
    /// </summary>
    [JsonProperty("club")]
    public APIClub Club { get; set; } = null!;

    /// <summary>
    /// The user this invite is for.
    /// </summary>
    [JsonProperty("user")]
    public APIUser User { get; set; } = null!;
}
