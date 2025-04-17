using System;
using fluXis.Map.Structures.Bases;
using fluXis.Screens.Gameplay.Ruleset.Playfields;
using Newtonsoft.Json;
using osu.Framework.Graphics;

namespace fluXis.Map.Structures.Events;

public class LayerFadeEvent : IMapEvent, IApplicableToPlayfield, IHasDuration, IHasEasing
{
    [JsonProperty("time")]
    public double Time { get; set; }

    [JsonProperty("duration")]
    public double Duration { get; set; }

    [JsonProperty("alpha")]
    public float Alpha { get; set; } = 1;

    [JsonProperty("ease")]
    public Easing Easing { get; set; }

    [JsonProperty("layer")]
    public FadeLayer Layer { get; set; } = FadeLayer.HitObjects;

    [JsonProperty("playfield")]
    public int PlayfieldIndex { get; set; }

    [JsonProperty("subfield")]
    public int PlayfieldSubIndex { get; set; }

    public void Apply(Playfield playfield)
    {
        if (!this.AppliesTo(playfield))
            return;

        Drawable drawable = Layer switch
        {
            FadeLayer.HitObjects => playfield.HitManager,
            FadeLayer.Stage => playfield.Stage,
            FadeLayer.Receptors => playfield.Receptors,
            FadeLayer.Playfield => playfield,
            _ => throw new ArgumentOutOfRangeException()
        };

        // make sure this is set, just in case it's missing
        if (Alpha <= 0.0001f)
            drawable.AlwaysPresent = true;

        using (drawable.BeginAbsoluteSequence(Time))
            drawable.FadeTo(Alpha, Math.Max(Duration, 0), Easing);
    }

    public enum FadeLayer
    {
        HitObjects,
        Stage,
        Receptors,
        Playfield
    }
}
