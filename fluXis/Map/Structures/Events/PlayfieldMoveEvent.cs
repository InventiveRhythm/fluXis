using fluXis.Map.Structures.Bases;
using fluXis.Screens.Gameplay.Ruleset.Playfields;
using Newtonsoft.Json;
using osu.Framework.Graphics;

namespace fluXis.Map.Structures.Events;

public class PlayfieldMoveEvent : IMapEvent, IHasDuration, IHasEasing, IApplicableToPlayfield
{
    [JsonProperty("time")]
    public double Time { get; set; }

    [JsonProperty("x")]
    public float OffsetX { get; set; }

    [JsonProperty("y")]
    public float OffsetY { get; set; }

    [JsonProperty("duration")]
    public double Duration { get; set; }

    [JsonProperty("ease")]
    public Easing Easing { get; set; } = Easing.OutQuint;

    [JsonProperty("playfield")]
    public int PlayfieldIndex { get; set; }

    public void Apply(Playfield playfield)
    {
        using (playfield.BeginAbsoluteSequence(Time))
        {
            playfield.MoveToX(OffsetX, Duration, Easing);
            playfield.MoveToY(OffsetY, Duration, Easing);
        }
    }
}
