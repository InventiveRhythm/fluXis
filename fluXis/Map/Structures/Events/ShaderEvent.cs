using System;
using fluXis.Graphics.Shaders;
using fluXis.Map.Structures.Bases;
using fluXis.Utils.Attributes;
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

        public void Set(int index, float val)
        {
            switch (index)
            {
                case 1:
                    Strength = val;
                    break;

                case 2:
                    Strength2 = val;
                    break;

                case 3:
                    Strength3 = val;
                    break;

                default:
                    throw new IndexOutOfRangeException();
            }
        }

        public float Get(int index) => index switch
        {
            1 => Strength,
            2 => Strength2,
            3 => Strength3,
            _ => throw new ArgumentOutOfRangeException(nameof(index), index, null)
        };
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

    [ShaderStrength(1, Max = 20f, Step = 1f)]
    Chromatic,
    Mosaic,
    Noise,
    Vignette,
    Retro,
    HueShift,

    [ShaderStrength(1, ParamName = "X Strength", Tooltip = "The strength of the glitch effect on the x-axis.")]
    [ShaderStrength(2, ParamName = "Y Strength", Tooltip = "The strength of the glitch effect on the y-axis.")]
    [ShaderStrength(3, ParamName = "Block Size", Tooltip = "The size of the glitch blocks.")]
    Glitch,

    [ShaderStrength(1)]
    [ShaderStrength(2, Max = 16f, Step = 1f, ParamName = "Splits X", Tooltip = "Amount of splits on X axis.", Single = true)]
    [ShaderStrength(3, Max = 16f, Step = 1f, ParamName = "Splits Y", Tooltip = "Amount of splits on Y axis.", Single = true)]
    SplitScreen,

    [ShaderStrength(1, Min = -1f)]
    FishEye,

    [ShaderStrength(1)]
    [ShaderStrength(2, ParamName = "Scale", Tooltip = "Scale factor of each consecutive reflection.")]
    Reflections
}
