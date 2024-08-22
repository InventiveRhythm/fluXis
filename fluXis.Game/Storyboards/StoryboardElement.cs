using System.Collections.Generic;
using fluXis.Game.Map.Structures.Bases;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using osu.Framework.Graphics;

namespace fluXis.Game.Storyboards;

public class StoryboardElement : ITimedObject
{
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
    public Anchor Anchor { get; set; }

    [JsonProperty("origin")]
    public Anchor Origin { get; set; }

    [JsonProperty("x")]
    public float StartX { get; set; }

    [JsonProperty("y")]
    public float StartY { get; set; }

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
}

public enum StoryboardElementType
{
    Box = 0,
    Sprite = 1,
    Text = 2
}

public enum StoryboardLayer
{
    Background = 0,
    Foreground = 1,
    Overlay = 2
}
