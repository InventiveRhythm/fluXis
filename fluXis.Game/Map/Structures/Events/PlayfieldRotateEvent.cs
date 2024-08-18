using fluXis.Game.Map.Structures.Bases;
using fluXis.Game.Screens.Gameplay.Ruleset;
using Newtonsoft.Json;
using osu.Framework.Graphics;

namespace fluXis.Game.Map.Structures.Events;

public class PlayfieldRotateEvent : IMapEvent, IHasDuration, IHasEasing, IApplicableToPlayfield
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

    public void Apply(Playfield playfield)
    {
        using (playfield.BeginAbsoluteSequence(Time))
            playfield.RotateTo(Roll, Duration, Easing);
    }
}
