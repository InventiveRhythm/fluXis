using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Map.Structures.Attributes;
using fluXis.Map.Structures.Bases;
using fluXis.Modes;
using fluXis.Screens.Edit.Tabs.Charting.Playfield;
using fluXis.Utils.Attributes;
using Midori.Utils.Extensions;
using Newtonsoft.Json;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Map.Structures.Events.Playfields;

[AnimatesProperty(nameof(Playfield.AnimationScale))]
[Description("Scales the playfield.")]
[Icon(FluXisIconType.PlayfieldScale)]
public class PlayfieldScaleEvent : IMapEvent, IHasDuration, IHasEasing, IApplicableToPlayfield
{
    [JsonProperty("time")]
    public double Time { get; set; }

    [JsonProperty("lane")]
    public int Lane { get; set; }

    [JsonProperty("group", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string Group { get; set; }

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

        using (playfield.BeginAbsoluteSequence(Time))
            playfield.TransformTo(nameof(playfield.AnimationScale), new Vector2(ScaleX, ScaleY), Math.Max(Duration, 0), Easing);
    }

    IEnumerable<Drawable> ITimedObject.CreateObjectOverlay(EditorDrawableObject obj)
    {
        var flow = (FillFlowContainer)ITimedObject.CreateDefaultOverlay(obj).First();
        flow.Add(ITimedObject.CreateSmallText(obj, () => $"{ScaleX.ToStringInvariant("0.00")}x{ScaleY.ToStringInvariant("0.00")}"));
        flow.Add(ITimedObject.CreateSmallText(obj, () => $"P{PlayfieldIndex}S{PlayfieldSubIndex}"));
        yield return flow;
    }
}
