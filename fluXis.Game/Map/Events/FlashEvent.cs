using fluXis.Game.Map.Structures;
using Newtonsoft.Json;
using osu.Framework.Graphics;
using osuTK.Graphics;

namespace fluXis.Game.Map.Events;

public class FlashEvent : ITimedObject, IHasDuration
{
    [JsonProperty("time")]
    public double Time { get; set; }

    [JsonProperty("duration")]
    public double Duration { get; set; }

    [JsonProperty("background")]
    public bool InBackground { get; set; }

    [JsonProperty("ease")]
    public Easing Easing { get; set; } = Easing.None;

    [JsonProperty("start-color")]
    public Color4 StartColor { get; set; } = Color4.White;

    [JsonProperty("start-alpha")]
    public float StartOpacity { get; set; } = 1;

    [JsonProperty("end-color")]
    public Color4 EndColor { get; set; } = Color4.White;

    [JsonProperty("end-alpha")]
    public float EndOpacity { get; set; }
}
