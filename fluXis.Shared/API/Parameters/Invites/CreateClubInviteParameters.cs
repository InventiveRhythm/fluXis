using Newtonsoft.Json;

namespace fluXis.Shared.API.Parameters.Invites;

public class CreateClubInviteParameters
{
    /// <summary>
    /// The user to invite.
    /// </summary>
    [JsonProperty("user")]
    public long? UserID { get; set; }
}
