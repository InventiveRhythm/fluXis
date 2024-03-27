using fluXis.Game.Map.Structures;
using Newtonsoft.Json;
using osu.Framework.Graphics;

namespace fluXis.Game.Map.Events;

public class PlayfieldScaleEvent : ITimedObject, IHasDuration
{
    [JsonProperty("time")]
    public float Time { get; set; }

    [JsonProperty("x")]
    public float ScaleX { get; set; }

    [JsonProperty("y")]
    public float ScaleY { get; set; }

    [JsonProperty("duration")]
    public float Duration { get; set; }

    [JsonProperty("ease")]
    public Easing Easing { get; set; }
}
