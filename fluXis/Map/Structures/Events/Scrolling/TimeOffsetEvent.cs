using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using fluXis.Map.Structures.Bases;
using fluXis.Modes.Keys.HitObjects;
using fluXis.Screens.Edit.Tabs.Charting.Playfield;
using Newtonsoft.Json;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Map.Structures.Events.Scrolling;

[Description("Offsets the hit objects visually.")]
public class TimeOffsetEvent : IMapEvent, IHasDuration, IHasEasing, IHasStartValue<double>, IApplicableToHitManager
{
    [JsonProperty("time")]
    public double Time { get; set; }

    [JsonProperty("lane")]
    public int Lane { get; set; }

    [JsonProperty("group", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string Group { get; set; }

    [JsonProperty("duration")]
    public double Duration { get; set; }

    [JsonProperty("use-start")]
    public bool UseStartValue { get; set; }

    [JsonProperty("start-offset")]
    public double StartOffset { get; set; }

    [JsonIgnore]
    public double StartValue
    {
        get => StartOffset;
        set => StartOffset = value;
    }

    [JsonProperty("offset")]
    public double TargetOffset { get; set; }

    [JsonProperty("ease")]
    public Easing Easing { get; set; }

    public void Apply(HitObjectManager manager)
    {
        using (manager.BeginAbsoluteSequence(Time))
        {
            if (UseStartValue)
                manager.TransformTo(nameof(manager.VisualTimeOffset), StartOffset);

            manager.TransformTo(nameof(manager.VisualTimeOffset), TargetOffset, Math.Max(Duration, 0), Easing);
        }
    }

    IEnumerable<Drawable> ITimedObject.CreateObjectOverlay(EditorDrawableObject obj)
    {
        var flow = (FillFlowContainer)ITimedObject.CreateDefaultOverlay(obj).First();
        flow.Add(ITimedObject.CreateSmallText(obj, () =>
        {
            var text = "";
            if (UseStartValue) text += $"{(int)StartOffset}ms > ";
            text += $"{(int)TargetOffset}ms";
            return text;
        }));
        flow.Add(ITimedObject.CreateSmallText(obj, () => Easing.ToString()));
        yield return flow;
    }
}
