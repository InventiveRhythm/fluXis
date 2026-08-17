using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using fluXis.Map.Structures.Bases;
using fluXis.Screens.Edit.Tabs.Charting.Playfield;
using fluXis.Screens.Gameplay.Ruleset.Playfields;
using Newtonsoft.Json;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osuTK.Graphics;

namespace fluXis.Map.Structures.Events;

[Description("Changes the colors of hit objects and other skin elements.")]
public class ColorFadeEvent : IMapEvent, IHasDuration, IHasEasing, IApplicableToPlayfield
{
    [JsonProperty("time")]
    public double Time { get; set; }

    [JsonProperty("lane")]
    public int Lane { get; set; }

    [JsonProperty("group", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string Group { get; set; }

    [JsonProperty("fade-primary")]
    public bool FadePrimary { get; set; }

    [JsonProperty("primary")]
    public Color4 Primary { get; set; } = Color4.White;

    [JsonProperty("fade-secondary")]
    public bool FadeSecondary { get; set; }

    [JsonProperty("secondary")]
    public Color4 Secondary { get; set; } = Color4.White;

    [JsonProperty("fade-middle")]
    public bool FadeMiddle { get; set; }

    [JsonProperty("middle")]
    public Color4 Middle { get; set; } = Color4.White;

    [JsonProperty("duration")]
    public double Duration { get; set; }

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

    public IEnumerable<Drawable> CreateObjectOverlay(EditorDrawableObject obj)
    {
        var p = new Box { Width = 12, RelativeSizeAxes = Axes.Y };
        yield return p;

        var s = new Box { Width = 12, RelativeSizeAxes = Axes.Y, X = 12 };
        yield return s;

        var m = new Box { Width = 12, RelativeSizeAxes = Axes.Y, X = 24 };
        yield return m;

        obj.DataUpdate += () =>
        {
            p.Colour = Primary;
            p.Alpha = FadePrimary ? 1 : 0;

            s.Colour = Secondary;
            s.Alpha = FadeSecondary ? 1 : 0;

            m.Colour = Middle;
            m.Alpha = FadeMiddle ? 1 : 0;
        };

        yield return ITimedObject.CreateDefaultOverlay(obj).First();
    }
}
