using fluXis.Map.Structures.Bases;
using fluXis.Screens.Gameplay.Ruleset.Playfields;
using Newtonsoft.Json;
using osu.Framework.Graphics;
using osuTK;

namespace fluXis.Map.Structures.Events;

public class PlayfieldScaleEvent : IMapEvent, IHasDuration, IHasEasing, IApplicableToPlayfield
{
    [JsonProperty("time")]
    public double Time { get; set; }

    [JsonProperty("x")]
    public float ScaleX { get; set; } = 1;

    [JsonProperty("y")]
    public float ScaleY { get; set; } = 1;

    [JsonProperty("duration")]
    public double Duration { get; set; }

    [JsonProperty("ease")]
    public Easing Easing { get; set; } = Easing.OutQuint;

    [JsonProperty("playfield")]
    public int PlayfieldIndex { get; set; }

    [JsonProperty("subfield")]
    public int PlayfieldSubIndex { get; set; }

    public void Apply(Playfield playfield)
    {
        if (!this.AppliesTo(playfield))
            return;

        var yScale = ScaleY;

        // invert if upscroll
        if (playfield.IsUpScroll)
            yScale *= -1;

        using (playfield.BeginAbsoluteSequence(Time))
            playfield.TransformTo(nameof(playfield.AnimationScale), new Vector2(ScaleX, yScale), Duration, Easing);
    }
}
