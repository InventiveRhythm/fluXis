using System.Collections.Generic;
using fluXis.Game.Online.API.Models.Users;
using Newtonsoft.Json;

namespace fluXis.Game.Online.API.Models.Clubs;

public class APIClub : APIClubShort
{
    [JsonProperty("owner")]
    public long OwnerID { get; set; }

    [JsonProperty("members")]
    public List<APIUserShort> Members { get; set; }
}
