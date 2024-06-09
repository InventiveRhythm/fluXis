using fluXis.Game.Map.Structures;
using Newtonsoft.Json;
using osu.Framework.Graphics;

namespace fluXis.Game.Map.Events;

public class PlayfieldRotateEvent : ITimedObject, IHasDuration
{
    [JsonProperty("time")]
    public double Time { get; set; }

    [JsonProperty("roll")]
    public float Roll { get; set; }

    // these are here for the future
    // I saw that one of the framework devs was working on
    // 3d rotation support, so I'm just going to leave these here
    /*[JsonProperty("pitch")]
    public float Pitch { get; set; }

    [JsonProperty("yaw")]
    public float Yaw { get; set; }*/

    [JsonProperty("duration")]
    public double Duration { get; set; }

    [JsonProperty("ease")]
    public Easing Easing { get; set; } = Easing.OutQuint;
}
