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

    public void Apply(Drawable drawable)
    {
        if (drawable is Playfield playfield && !this.AppliesTo(playfield))
            return;

        // make sure this is set, just in case it's missing
        if (Alpha <= 0.0001f)
            drawable.AlwaysPresent = true;

        using (drawable.BeginAbsoluteSequence(Time))
            drawable.FadeTo(Alpha, Duration, Easing);
    }

    public enum FadeLayer
    {
        HitObjects,
        Stage,
        Receptors,
        Playfield
    }

    void IApplicableToPlayfield.Apply(Playfield playfield) => Apply(playfield);
}
