using fluXis.Shared.Components.Other;
using fluXis.Shared.Components.Users;
using Newtonsoft.Json;

namespace fluXis.Shared.Components.Clubs;

public class APIClub
{
    [JsonProperty("id")]
    public long ID { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; } = "";

    [JsonProperty("tag")]
    public string Tag { get; set; } = "";

    [JsonProperty("colors")]
    public List<GradientColor> Colors { get; set; } = new();

    #region Optional

    [JsonProperty("owner")]
    public APIUser? Owner { get; set; }

    [JsonProperty("join-type")]
    public ClubJoinType? JoinType { get; set; }

    [JsonProperty("members")]
    public List<APIUser>? Members { get; set; }

    [JsonProperty("stats")]
    public APIClubStatistics? Statistics { get; set; }

    #endregion
}
