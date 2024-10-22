using fluXis.Game.Map.Structures.Bases;
using fluXis.Game.Screens.Gameplay.Ruleset.Playfields;
using Newtonsoft.Json;
using osu.Framework.Graphics;

namespace fluXis.Game.Map.Structures.Events;

public class PlayfieldFadeEvent : IMapEvent, IHasDuration, IHasEasing, IApplicableToPlayfield
{
    [JsonProperty("time")]
    public double Time { get; set; }

    [JsonProperty("duration")]
    public double Duration { get; set; }

    [JsonProperty("alpha")]
    public float Alpha { get; set; } = 1;

    [JsonProperty("ease")]
    public Easing Easing { get; set; }

    public void Apply(Playfield playfield)
    {
        using (playfield.BeginAbsoluteSequence(Time))
            playfield.FadeTo(Alpha, Duration, Easing);
    }
}
