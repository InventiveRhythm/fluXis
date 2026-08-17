using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using fluXis.Map.Structures.Bases;
using fluXis.Screens.Edit.Tabs.Charting.Playfield;
using fluXis.Screens.Gameplay.Ruleset;
using Midori.Utils.Extensions;
using Newtonsoft.Json;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Map.Structures.Events.Scrolling;

[Description("Adjusts scroll speed directly.")]
public class ScrollMultiplierEvent : IMapEvent, IHasDuration, IHasEasing, IHasGroups
{
    [JsonProperty("time")]
    public double Time { get; set; }

    [JsonProperty("lane")]
    public int Lane { get; set; }

    [JsonProperty("group", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string Group { get; set; }

    [JsonProperty("duration")]
    public double Duration { get; set; }

    [JsonProperty("multiplier")]
    public float Multiplier { get; set; } = 1;

    [JsonProperty("ease")]
    public Easing Easing { get; set; }

    [JsonProperty("groups", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public List<string> Groups { get; set; } = new();

    public void Apply(ScrollGroup group)
    {
        using (group.BeginAbsoluteSequence(Time))
            group.TransformTo(nameof(group.ScrollMultiplier), Multiplier, Math.Max(Duration, 0), Easing);
    }

    IEnumerable<Drawable> ITimedObject.CreateObjectOverlay(EditorDrawableObject obj)
    {
        var flow = (FillFlowContainer)ITimedObject.CreateDefaultOverlay(obj).First();
        flow.Add(ITimedObject.CreateSmallText(obj, () => $"{Multiplier.ToStringInvariant("0.##")}x"));
        flow.Add(ITimedObject.CreateSmallText(obj, () => Easing.ToString()));
        yield return flow;
    }
}
