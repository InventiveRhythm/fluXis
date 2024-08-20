using fluXis.Shared.Components.Clubs;
using Newtonsoft.Json;

namespace fluXis.Shared.API.Payloads.Clubs;

public class EditClubPayload
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
