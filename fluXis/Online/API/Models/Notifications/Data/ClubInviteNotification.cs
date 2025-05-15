using fluXis.Online.API.Models.Clubs;
using Newtonsoft.Json;

namespace fluXis.Online.API.Models.Notifications.Data;

public class ClubInviteNotification
{
    [JsonProperty("code")]
    public string InviteCode { get; set; }

    [JsonProperty("club")]
    public APIClub Club { get; set; }
}
