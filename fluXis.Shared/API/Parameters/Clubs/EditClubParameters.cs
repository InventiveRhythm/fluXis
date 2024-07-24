using fluXis.Shared.Components.Clubs;
using Newtonsoft.Json;

namespace fluXis.Shared.API.Parameters.Clubs;

public class EditClubParameters
{
    /// <summary>
    /// 3-16 characters. has to be unique
    /// </summary>
    [JsonProperty("name")]
    public string? Name { get; set; }

    /// <summary>
    /// the join type of the club
    /// </summary>
    [JsonProperty("join-type")]
    public ClubJoinType? JoinType { get; set; }

    /// <summary>
    /// Hex color code
    /// </summary>
    [JsonProperty("color-start")]
    public string? ColorStart { get; set; }

    /// <summary>
    /// Hex color code
    /// </summary>
    [JsonProperty("color-end")]
    public string? ColorEnd { get; set; }
}
