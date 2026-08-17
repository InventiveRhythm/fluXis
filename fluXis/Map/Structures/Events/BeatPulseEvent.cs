using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using fluXis.Map.Structures.Bases;
using fluXis.Screens.Edit.Tabs.Charting.Playfield;
using Midori.Utils.Extensions;
using Newtonsoft.Json;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Map.Structures.Events;

[Description("Zooms in and out to the beat of the song.")]
public class BeatPulseEvent : IMapEvent
{
    [JsonProperty("time")]
    public double Time { get; set; }

    [JsonProperty("lane")]
    public int Lane { get; set; }

    [JsonProperty("group", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string Group { get; set; }

    [JsonProperty("strength")]
    public float Strength { get; set; } = 1.05f;

    /// <summary>
    /// How much of the length should be used to zoom in. (in %)
    /// </summary>
    [JsonProperty("zoom")]
    public float ZoomIn { get; set; } = .25f;

    [JsonProperty("interval")]
    public float Interval { get; set; } = 1;

    IEnumerable<Drawable> ITimedObject.CreateObjectOverlay(EditorDrawableObject obj)
    {
        var flow = (FillFlowContainer)ITimedObject.CreateDefaultOverlay(obj).First();
        flow.Add(ITimedObject.CreateSmallText(obj, () => $"{Strength.ToStringInvariant("0.00")}x {Interval.ToStringInvariant("0.##")}/beat"));
        yield return flow;
    }
}
