using System;
using fluXis.Map.Structures.Bases;
using fluXis.Screens.Gameplay.Ruleset.Playfields;
using Midori.Logging;
using Newtonsoft.Json;
using osu.Framework.Graphics;
using osuTK.Graphics;

namespace fluXis.Map.Structures.Events;

public class ColorFadeEvent : IMapEvent, IHasDuration, IHasEasing, IApplicableToPlayfield
{
    [JsonProperty("time")]
    public double Time { get; set; }

    [JsonProperty("fade-primary")]
    public bool FadePrimary { get; set; } = false;

    [JsonProperty("primary")]
    public Color4 Primary { get; set; } = Color4.White;

    [JsonProperty("fade-secondary")]
    public bool FadeSecondary { get; set; } = false;

    [JsonProperty("secondary")]
    public Color4 Secondary { get; set; } = Color4.White;

    [JsonProperty("fade-middle")]
    public bool FadeMiddle { get; set; } = false;

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
            if (FadePrimary)
                manager.TransformTo(nameof(ColorManager.Primary), (Colour4)Primary, Math.Max(Duration, 0), Easing);
            if (FadeSecondary)
                manager.TransformTo(nameof(ColorManager.Secondary), (Colour4)Secondary, Math.Max(Duration, 0), Easing);
            if (FadeMiddle)
                manager.TransformTo(nameof(ColorManager.Middle), (Colour4)Middle, Math.Max(Duration, 0), Easing);
        }
    }
}
