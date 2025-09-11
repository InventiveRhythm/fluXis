using System;
using fluXis.Map.Structures.Bases;
using fluXis.Screens.Gameplay.Ruleset.Playfields;
using Newtonsoft.Json;
using osu.Framework.Graphics;
using osuTK.Graphics;

namespace fluXis.Map.Structures.Events;

public class ColorFadeEvent : IMapEvent, IHasDuration, IHasEasing, IApplicableToPlayfield
{
    [JsonProperty("time")]
    public double Time { get; set; }

    [JsonProperty("primary")]
    public Color4 Primary { get; set; } = Color4.White;

    [JsonProperty("secondary")]
    public Color4 Secondary { get; set; } = Color4.White;

    [JsonProperty("middle")]
    public Color4 Middle { get; set; } = Color4.White;

    [JsonProperty("duration")]
    public double Duration { get; set; } = 0;

    [JsonProperty("ease")]
    public Easing Easing { get; set; } = Easing.None;

    [JsonProperty("playfield")]
    public int PlayfieldIndex { get; set; }

    [JsonProperty("subfield")]
    public int PlayfieldSubIndex { get; set; }

    public void Apply(Playfield playfield)
    {
        if (!this.AppliesTo(playfield))
            return;

        using (playfield.BeginAbsoluteSequence(Time))
        {
            var manager = playfield.ColorManager;
            manager.TransformTo(nameof(ColorManager.Primary), (Colour4)Primary, Math.Max(Duration, 0), Easing);
            manager.TransformTo(nameof(ColorManager.Secondary), (Colour4)Secondary, Math.Max(Duration, 0), Easing);
            manager.TransformTo(nameof(ColorManager.Middle), (Colour4)Middle, Math.Max(Duration, 0), Easing);
        }
    }
}
