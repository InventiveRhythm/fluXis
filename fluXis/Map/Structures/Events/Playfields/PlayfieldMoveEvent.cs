using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Map.Structures.Attributes;
using fluXis.Map.Structures.Bases;
using fluXis.Screens.Edit.Tabs.Charting.Playfield;
using fluXis.Screens.Gameplay.Ruleset.Playfields;
using fluXis.Utils.Attributes;
using Newtonsoft.Json;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Map.Structures.Events.Playfields;

[AnimatesProperty(nameof(Playfield.AnimationX))]
[AnimatesProperty(nameof(Playfield.AnimationY))]
[AnimatesProperty(nameof(Playfield.AnimationZ))]
[Description("Moves the playfield.")]
[Icon(FluXisIconType.PlayfieldMove)]
public class PlayfieldMoveEvent : IMapEvent, IHasDuration, IHasEasing, IApplicableToPlayfield
{
    [JsonProperty("time")]
    public double Time { get; set; }

    [JsonProperty("lane")]
    public int Lane { get; set; }

    [JsonProperty("group", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string Group { get; set; }

    [JsonProperty("x")]
    public float OffsetX { get; set; }

    [JsonProperty("y")]
    public float OffsetY { get; set; }

    [JsonProperty("z")]
    public float OffsetZ { get; set; }

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
        {
            playfield.TransformTo(nameof(playfield.AnimationX), OffsetX, Math.Max(Duration, 0), Easing);
            playfield.TransformTo(nameof(playfield.AnimationY), OffsetY, Math.Max(Duration, 0), Easing);
            playfield.TransformTo(nameof(playfield.AnimationZ), OffsetZ, Math.Max(Duration, 0), Easing);
        }
    }

    IEnumerable<Drawable> ITimedObject.CreateObjectOverlay(EditorDrawableObject obj)
    {
        var flow = (FillFlowContainer)ITimedObject.CreateDefaultOverlay(obj).First();
        flow.Add(ITimedObject.CreateSmallText(obj, () => $"{(int)OffsetX}x {(int)OffsetY}y {(int)OffsetZ}z"));
        flow.Add(ITimedObject.CreateSmallText(obj, () => $"P{PlayfieldIndex}S{PlayfieldSubIndex}"));
        yield return flow;
    }
}
