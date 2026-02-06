using System.Collections.Generic;
using fluXis.Map.Structures.Bases;
using fluXis.Utils.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using osu.Framework.Graphics;

namespace fluXis.Storyboards;

public class StoryboardElement : ITimedObject
{
    /// <summary>
    /// Custom label for this element, only gets used in the editor.
    /// </summary>
    [JsonProperty("label")]
    public string Label { get; set; } = string.Empty;

    /// <summary>
    /// The type of the element.
    /// </summary>
    [JsonProperty("type")]
    public StoryboardElementType Type { get; set; }

    /// <summary>
    /// The layer of the element.
    /// </summary>
    [JsonProperty("layer")]
    public StoryboardLayer Layer { get; set; } = StoryboardLayer.Background;

    /// <summary>
    /// The depth of the element.
    /// </summary>
    [JsonProperty("z-index")]
    public int ZIndex { get; set; }

    /// <summary>
    /// The start time of the element.
    /// Spawns the element at this time.
    /// </summary>
    [JsonProperty("start")]
    public double StartTime { get; set; }

    /// <summary>
    /// The end time of the element.
    /// Despawns the element at this time.
    /// </summary>
    [JsonProperty("end")]
    public double EndTime { get; set; }

    [JsonProperty("anchor")]
    public Anchor Anchor { get; set; } = Anchor.TopLeft;

    [JsonProperty("origin")]
    public Anchor Origin { get; set; } = Anchor.TopLeft;

    [JsonProperty("x")]
    public float StartX { get; set; }

    [JsonProperty("y")]
    public float StartY { get; set; }

    [JsonProperty("blend")]
    public bool Blending { get; set; }

    [JsonProperty("blend-mode")]
    public DefaultBlendingParameters BlendingMode { get; set; } = DefaultBlendingParameters.Add;

    [JsonProperty("width")]
    public float Width { get; set; }

    [JsonProperty("height")]
    public float Height { get; set; }

    [JsonProperty("color")]
    public uint Color { get; set; } = 0xFFFFFFFF;

    [JsonProperty("parameters")]
    public Dictionary<string, JToken> Parameters { get; set; } = new();

    [JsonProperty("animations")]
    public List<StoryboardAnimation> Animations { get; set; } = new();

    [JsonIgnore]
    double ITimedObject.Time { get => StartTime; set => StartTime = value; }

    public T GetParameter<T>(string key, T fallback)
    {
        if (!Parameters.TryGetValue(key, out var token))
            return fallback;

        try
        {
            return token.ToObject<T>() ?? fallback;
        }
        catch
        {
            return fallback;
        }
    }
}

public enum StoryboardElementType
{
    [Icon(0xf0c8)]
    [WidthHeight]
    [AllowedAnimation(StoryboardAnimationType.Width)]
    [AllowedAnimation(StoryboardAnimationType.Height)]
    Box = 0,

    [Icon(0xf03e)]
    Sprite = 1,

    [Icon(0xf031)]
    Text = 2,

    [Icon(0xf70e)]
    Script = 3,

    [Icon(0xf111)]
    [WidthHeight]
    [AllowedAnimation(StoryboardAnimationType.Width)]
    [AllowedAnimation(StoryboardAnimationType.Height)]
    [AllowedAnimation(StoryboardAnimationType.Rotate, true)]
    Circle = 4,

    [Icon(0xf111, Regular = true)]
    [WidthHeight]
    [AllowedAnimation(StoryboardAnimationType.Width)]
    [AllowedAnimation(StoryboardAnimationType.Height)]
    [AllowedAnimation(StoryboardAnimationType.Border)]
    [AllowedAnimation(StoryboardAnimationType.Rotate, true)]
    OutlineCircle = 5,

    [Icon(0xf1fc)]
    [WidthHeight]
    [AllowedAnimation(StoryboardAnimationType.Width)]
    [AllowedAnimation(StoryboardAnimationType.Height)]
    SkinSprite = 6,

    [Icon(0xf0c8, Regular = true)]
    [WidthHeight]
    [AllowedAnimation(StoryboardAnimationType.Width)]
    [AllowedAnimation(StoryboardAnimationType.Height)]
    [AllowedAnimation(StoryboardAnimationType.Border)]
    OutlineBox = 7,
}

public enum StoryboardLayer
{
    Background = 0,
    Foreground = 1,
    Overlay = 2
}

public enum SkinSprite
{
    HitObject,
    LongNoteStart,
    LongNoteBody,
    LongNoteEnd,
    TickNote,
    TickNoteSmall,
    Receptor,

    StageBackground,
    StageBackgroundTop,
    StageBackgroundBottom,
    StageLeftTop,
    StageLeft,
    StageLeftBottom,
    StageRightTop,
    StageRight,
    StageRightBottom,
}
