using fluXis.Game.Map.Structures.Bases;
using Newtonsoft.Json;
using osu.Framework.Graphics;

namespace fluXis.Game.Map.Structures;

public class HitSoundFade : ITimedObject
{
    /// <summary>
    /// The time at which the volume change should start.
    /// </summary>
    [JsonProperty("time")]
    public double Time { get; set; }

    /// <summary>
    /// The sound to change the volume of.
    /// </summary>
    [JsonProperty("sound")]
    public string HitSound { get; set; }

    /// <summary>
    /// The volume to fade to.
    /// </summary>
    [JsonProperty("volume")]
    public double Volume { get; set; }

    /// <summary>
    /// The duration of the fade.
    /// </summary>
    [JsonProperty("duration")]
    public float Duration { get; set; }

    /// <summary>
    /// The easing function to use for the fade.
    /// </summary>
    [JsonProperty("ease")]
    public Easing Easing { get; set; }
}
