using System;
using fluXis.Graphics.Shaders;
using fluXis.Map.Structures.Bases;
using fluXis.Utils.Extensions;
using Newtonsoft.Json;
using osu.Framework.Graphics;
using osu.Framework.Logging;

namespace fluXis.Map.Structures.Events;

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

    [JsonProperty("use-start")]
    public bool UseStartParams { get; set; }

    [JsonProperty("start-params")]
    public ShaderParameters StartParameters { get; set; } = new();

    [JsonProperty("end-params")]
    public ShaderParameters EndParameters { get; set; } = new();

    [JsonProperty("params")]
    [Obsolete($"Use {nameof(EndParameters)} instead.")]
    public ShaderParameters Parameters { set => EndParameters = value; }

    public class ShaderParameters
    {
        [JsonProperty("strength")]
        public float Strength { get; set; }

        [JsonProperty("strength2")]
        public float Strength2 { get; set; }

        [JsonProperty("strength3")]
        public float Strength3 { get; set; }
    }

    public void Apply(ShaderTransformHandler shader)
    {
        using (shader.BeginAbsoluteSequence(Time))
        {
            if (UseStartParams)
            {
                shader.StrengthTo(StartParameters.Strength);
                shader.Strength2To(StartParameters.Strength2);
                shader.Strength3To(StartParameters.Strength3);
            }

            shader.StrengthTo(EndParameters.Strength, Math.Max(Duration, 0), Easing);
            shader.Strength2To(EndParameters.Strength2, Math.Max(Duration, 0), Easing);
            shader.Strength3To(EndParameters.Strength3, Math.Max(Duration, 0), Easing);
        }
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
    Retro,
    HueShift,
    Glitch,
    SplitScreen
}
