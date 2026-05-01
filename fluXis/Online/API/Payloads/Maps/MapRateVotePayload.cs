using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using fluXis.Utils.Attributes;
using Newtonsoft.Json;

namespace fluXis.Online.API.Payloads.Maps;

// TODO: make fluxel use this instead of MapRateVoteRoute's internal 'Payload' class
public class MapRateVotePayload
{
    [Description("Chart Difficulty (0-20)")]
    [Tooltip("The base chart difficulty.")]
    [Placeholder("0")]
    [Range(0.0, 20.0)]
    [JsonProperty("base")]
    public float Base { get; set; }

    [Description("Read Difficulty (0-5)")]
    [Tooltip("SV stuff.")]
    [Placeholder("0")]
    [Range(0.0, 5.0)]
    [JsonProperty("read")]
    public float Reading { get; set; }

    [Description("Track Difficulty (0-5)")]
    [Tooltip("uuuh.")]
    [Placeholder("0")]
    [Range(0.0, 5.0)]
    [JsonProperty("track")]
    public float Tracking { get; set; }

    [Description("Perception Difficulty (0-5)")]
    [Tooltip("Blocking elements such as shaders and storyboard.")]
    [Placeholder("0")]
    [Range(0.0, 5.0)]
    [JsonProperty("percept")]
    public float Perception { get; set; }
}
