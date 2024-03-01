using fluXis.Game.Utils;
using Newtonsoft.Json;
using osu.Framework.Graphics;
using osuTK;

namespace fluXis.Game.Storyboards;

public class StoryboardAnimation
{
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
    /// The type of the animation.
    /// </summary>
    [JsonProperty("type")]
    public StoryboardAnimationType Type { get; set; }

    /// <summary>
    /// The start value of the animation.
    /// </summary>
    [JsonProperty("start")]
    public string ValueStart { get; set; }

    /// <summary>
    /// The end value of the animation.
    /// </summary>
    [JsonProperty("end")]
    public string ValueEnd { get; set; }

    [JsonIgnore]
    public float StartFloat => ValueStart.ToFloatInvariant();

    [JsonIgnore]
    public float EndFloat => ValueEnd.ToFloatInvariant();

    [JsonIgnore]
    public Vector2 StartVector
    {
        get
        {
            var xy = ValueStart.Split(',');
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
}

public enum StoryboardAnimationType
{
    MoveX = 0,
    MoveY = 1,
    Scale = 2,
    ScaleVector = 3,
    Width = 4,
    Height = 5,
    Rotate = 6,
    Fade = 7,
    Color = 8
}
