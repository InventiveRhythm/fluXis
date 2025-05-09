using fluXis.Online.API.Models.Clubs;
using Newtonsoft.Json;

namespace fluXis.Online.API.Models.Other;

public class APIInvite
{
    [JsonProperty("code")]
    public string InviteCode { get; set; }

    [JsonProperty("club")]
    public APIClub TargetClub { get; set; }

    [JsonProperty("user")]
    public long IntendedUser { get; set; }
}
