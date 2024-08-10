using fluXis.Shared.Components.Clubs;
using Newtonsoft.Json;

namespace fluXis.Shared.API.Parameters.Clubs;

public class CreateClubParameters
{
    /// <summary>
    /// 3-16 characters. has to be unique
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; set; } = null!;

    /// <summary>
    /// 3-5 characters. has to be unique
    /// </summary>
    [JsonProperty("tag")]
    public string Tag { get; set; } = null!;

    /// <summary>
    /// B64-encoded image
    /// </summary>
    [JsonProperty("icon")]
    public string Icon { get; set; } = null!;

    /// <summary>
    /// B64-encoded image
    /// </summary>
    [JsonProperty("banner")]
    public string Banner { get; set; } = null!;

    /// <summary>
    /// the join type of the club
    /// </summary>
    [JsonProperty("join-type")]
    public ClubJoinType JoinType { get; set; }

    /// <summary>
    /// Hex color code
    /// </summary>
    [JsonProperty("color-start")]
    public string ColorStart { get; set; } = null!;

    /// <summary>
    /// Hex color code
    /// </summary>
    [JsonProperty("color-end")]
    public string ColorEnd { get; set; } = null!;
}
