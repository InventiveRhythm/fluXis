using System.ComponentModel.DataAnnotations;
using fluXis.Online.API.Models.Clubs;
using fluXis.Utils;
using Newtonsoft.Json;

namespace fluXis.Online.API.Payloads.Clubs;

public class CreateClubPayload
{
    [JsonProperty("name")]
    [Required, StringLength(24, MinimumLength = 3)]
    public string Name { get; set; } = null!;

    /// <summary>
    /// 3-5 characters. has to be unique
    /// </summary>
    [JsonProperty("tag")]
    [Required, RegularExpression("^[A-Z0-9]{3,5}$")]
    public string Tag { get; set; } = null!;

    /// <summary>
    /// B64-encoded image
    /// </summary>
    [JsonProperty("icon")]
    [Base64String]
    public string Icon { get; set; } = null!;

    /// <summary>
    /// B64-encoded image
    /// </summary>
    [JsonProperty("banner")]
    [Base64String]
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
    [Required, RegularExpression(Validate.REGEX_HEX_COLOR)]
    public string ColorStart { get; set; } = null!;

    /// <summary>
    /// Hex color code
    /// </summary>
    [JsonProperty("color-end")]
    [Required, RegularExpression(Validate.REGEX_HEX_COLOR)]
    public string ColorEnd { get; set; } = null!;
}
