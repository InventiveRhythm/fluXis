using fluXis.Game.Map.Structures;
using Newtonsoft.Json;

namespace fluXis.Game.Map.Events;

public class ShaderEvent : ITimedObject, IHasDuration
{
    [JsonProperty("time")]
    public double Time { get; set; }

    [JsonProperty("shader")]
    public string ShaderName { get; set; } = string.Empty;

    [JsonProperty("duration")]
    public double Duration { get; set; }

    [JsonProperty("params")]
    public ShaderParameters Parameters { get; set; } = new();

    public static string[] ShaderNames =
    {
        "Bloom",
        "Greyscale",
        "Invert",
        "Chromatic",
        "Mosaic",
        "Noise",
        "Vignette",
        "Retro"
    };

    public class ShaderParameters
    {
        [JsonProperty("strength")]
        public float Strength { get; set; }
    }
}
