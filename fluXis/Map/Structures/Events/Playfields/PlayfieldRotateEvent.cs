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
using Midori.Utils.Extensions;
using Newtonsoft.Json;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Map.Structures.Events.Playfields;

[AnimatesProperty(nameof(Playfield.Rotation))]
[Description("Rotates the playfield.")]
[Icon(FluXisIconType.PlayfieldRotate)]
public class PlayfieldRotateEvent : IMapEvent, IHasDuration, IHasEasing, IApplicableToPlayfield
{
    [JsonProperty("time")]
    public double Time { get; set; }

    [JsonProperty("lane")]
    public int Lane { get; set; }

    [JsonProperty("group", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string Group { get; set; }

    [JsonProperty("roll")]
    public float Roll { get; set; }

    // these are here for the future
    // I saw that one of the framework devs was working on
    // 3d rotation support, so I'm just going to leave these here
    /*[JsonProperty("pitch")]
    public float Pitch { get; set; }

    [JsonProperty("yaw")]
    public float Yaw { get; set; }*/

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
            playfield.RotateTo(Roll, Math.Max(Duration, 0), Easing);
    }

    IEnumerable<Drawable> ITimedObject.CreateObjectOverlay(EditorDrawableObject obj)
    {
        var flow = (FillFlowContainer)ITimedObject.CreateDefaultOverlay(obj).First();
        flow.Add(ITimedObject.CreateSmallText(obj, () => $"{Roll.ToStringInvariant("0")}deg"));
        flow.Add(ITimedObject.CreateSmallText(obj, () => $"P{PlayfieldIndex}S{PlayfieldSubIndex}"));
        yield return flow;
    }
}
