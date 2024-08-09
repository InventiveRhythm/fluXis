using System;
using fluXis.Game.Map.Structures.Bases;
using Newtonsoft.Json;
using osu.Framework.Graphics;
using osu.Framework.Logging;

namespace fluXis.Game.Map.Structures.Events;

public class ShaderEvent : IMapEvent, IHasDuration, IHasEasing
{
    [JsonProperty("time")]
    public double Time { get; set; }

    [JsonProperty("shader")]
    public string ShaderName
    {
        get => Type.ToString();
        set
        {
            if (Enum.TryParse<ShaderType>(value, out var type))
                Type = type;
            else
                Logger.Log($"Failed to parse {value} as {nameof(ShaderType)}!", LoggingTarget.Runtime, LogLevel.Error);
        }
    }

    [JsonIgnore]
    public ShaderType Type { get; set; } = ShaderType.Bloom;

    [JsonProperty("duration")]
    public double Duration { get; set; }

    [JsonProperty("ease")]
    public Easing Easing { get; set; }

    [JsonProperty("params")]
    public ShaderParameters Parameters { get; set; } = new();

    public class ShaderParameters
    {
        [JsonProperty("strength")]
        public float Strength { get; set; }
    }
}

public enum ShaderType
{
    Bloom,
    Greyscale,
    Invert,
    Chromatic,
    Mosaic,
    Noise,
    Vignette,
    Retro
}
