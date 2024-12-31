using Newtonsoft.Json;

namespace fluXis.Online.API.Payloads.Invites;

public class CreateClubInvitePayload
{
    /// <summary>
    /// The user to invite.
    /// </summary>
    [JsonProperty("user")]
    public long? UserID { get; set; }
}
