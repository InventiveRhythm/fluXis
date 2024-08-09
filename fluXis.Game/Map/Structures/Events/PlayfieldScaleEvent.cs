using fluXis.Game.Map.Structures.Bases;
using fluXis.Game.Screens.Gameplay.Ruleset;
using Newtonsoft.Json;
using osu.Framework.Graphics;
using osuTK;

namespace fluXis.Game.Map.Structures.Events;

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

    public void Apply(Playfield playfield)
    {
        var yScale = ScaleY;

        // invert if upscroll
        if (playfield.IsUpScroll)
            yScale *= -1;

        playfield.ScaleTo(new Vector2(ScaleX, yScale), Duration, Easing);
    }
}
