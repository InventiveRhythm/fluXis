using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Map.Structures.Bases;
using fluXis.Screens.Edit.Tabs.Charting.Playfield;
using fluXis.Utils.Attributes;
using Midori.Utils.Extensions;
using Newtonsoft.Json;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Map.Structures.Events;

[Description("Shakes the screen.")]
[Icon(FluXisIconType.Shake)]
public class ShakeEvent : IMapEvent, IHasDuration
{
    [JsonProperty("time")]
    public double Time { get; set; }

    [JsonProperty("lane")]
    public int Lane { get; set; }

    [JsonProperty("group", DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string Group { get; set; }

    [JsonProperty("duration")]
    public double Duration { get; set; }

    [JsonProperty("magnitude")]
    public float Magnitude { get; set; } = 10;

    IEnumerable<Drawable> ITimedObject.CreateObjectOverlay(EditorDrawableObject obj)
    {
        var flow = (FillFlowContainer)ITimedObject.CreateDefaultOverlay(obj).First();
        flow.Add(ITimedObject.CreateSmallText(obj, () => $"{Magnitude.ToStringInvariant("0")}px"));
        yield return flow;
    }
}
