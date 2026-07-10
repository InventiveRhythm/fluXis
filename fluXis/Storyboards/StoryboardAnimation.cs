using System.ComponentModel;
using fluXis.Map.Structures.Bases;
using Midori.Utils.Extensions;
using Newtonsoft.Json;
using osu.Framework.Graphics;
using osuTK;
using SixLabors.ImageSharp;

namespace fluXis.Storyboards;

public class StoryboardAnimation : ITimedObject, IHasDuration, IHasEasing, IHasStartValue<string>, IDeepCloneable<StoryboardAnimation>
{
    public StoryboardAnimation(StoryboardElement parentElement)
    {
        ParentElement = parentElement;
    }

    /// <summary>
    /// The start time of the animation.
    /// </summary>
    [JsonProperty("start")]
    public double StartTime { get; set; }

    [JsonIgnore]
    public double EndTime => StartTime + Duration;

    /// <summary>
    /// The duration of the animation.
    /// </summary>
    [JsonProperty("duration")]
    public double Duration { get; set; }

    /// <summary>
    /// The easing of the animation.
    /// </summary>
    [JsonProperty("easing")]
    public Easing Easing { get; set; }

    /// <summary>
    /// Whether to override the start value
    /// </summary>
    [JsonProperty("use-start", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public bool UseStartValue { get; set; } = true;

    /// <summary>
    /// The type of the animation.
    /// </summary>
    [JsonProperty("type")]
    public StoryboardAnimationType Type { get; set; }

    /// <summary>
    /// The start value of the animation.
    /// </summary>
    [JsonProperty("start-value")]
    public string StartValue { get; set; }

    /// <summary>
    /// The end value of the animation.
    /// </summary>
    [JsonProperty("end-value")]
    public string ValueEnd { get; set; }

    [JsonIgnore]
    public float StartFloat => StartValue.ToFloatInvariant();

    [JsonIgnore]
    public float EndFloat => ValueEnd.ToFloatInvariant();

    [JsonIgnore]
    public Vector2 StartVector
    {
        get
        {
            var xy = StartValue.Split(',');
            return new Vector2(xy[0].ToFloatInvariant(), xy[1].ToFloatInvariant());
        }
    }

    [JsonIgnore]
    public Vector2 EndVector
    {
        get
        {
            var xy = ValueEnd.Split(',');
            return new Vector2(xy[0].ToFloatInvariant(), xy[1].ToFloatInvariant());
        }
    }

    [JsonIgnore]
    public StoryboardElement ParentElement;

    public StoryboardAnimation DeepClone() => new(ParentElement)
    {
        StartTime = StartTime,
        Duration = Duration,
        Easing = Easing,
        Type = Type,
        StartValue = StartValue,
        ValueEnd = ValueEnd
    };

    [JsonIgnore]
    double ITimedObject.Time
    {
        get => StartTime;
        set => StartTime = value;
    }

    [JsonIgnore]
    string ITimedObject.Group { get; set; }
}

public enum StoryboardAnimationType
{
    [Description("X Position")]
    MoveX = 0,

    [Description("Y Position")]
    MoveY = 1,
    Scale = 2,

    [Description("Vector Scale")]
    ScaleVector = 3,
    Width = 4,
    Height = 5,

    [Description("Rotation")]
    Rotate = 6,

    [Description("Alpha")]
    Fade = 7,
    Color = 8,
    Border = 9
}
