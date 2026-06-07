using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using fluXis.Online.API.Models.Clubs;
using fluXis.Utils.Attributes;
using Newtonsoft.Json;
using osu.Framework.Graphics.Containers;

namespace fluXis.Online.API.Payloads.Clubs;

#nullable enable

public class EditClubPayload
{
    [MaxLength(24)]
    [JsonProperty("name")]
    public string? Name { get; set; }

    /// <summary>
    /// the join type of the club
    /// </summary>
    [Hidden]
    [JsonProperty("join-type")]
    public ClubJoinType? JoinType { get; set; }

    /// <summary>
    /// B64-encoded image
    /// </summary>
    [Group("assets", SizeMode = GridSizeMode.Absolute, Size = 128, Aspect = 1f)]
    [JsonProperty("icon")]
    [TypeOverride(TypeOverrideAttribute.Type.Image)]
    public string? Icon { get; set; }

    /// <summary>
    /// B64-encoded image
    /// </summary>
    [Group("assets", RelativeHeight = true)]
    [JsonProperty("banner")]
    [TypeOverride(TypeOverrideAttribute.Type.Image)]
    public string? Banner { get; set; }

    /// <summary>
    /// Hex color code
    /// </summary>
    [Group("gradient")]
    [Description("Start Color")]
    [JsonProperty("color-start")]
    [TypeOverride(TypeOverrideAttribute.Type.Color)]
    public string? ColorStart { get; set; }

    /// <summary>
    /// Hex color code
    /// </summary>
    [Group("gradient")]
    [Description("End Color")]
    [JsonProperty("color-end")]
    [TypeOverride(TypeOverrideAttribute.Type.Color)]
    public string? ColorEnd { get; set; }
}
