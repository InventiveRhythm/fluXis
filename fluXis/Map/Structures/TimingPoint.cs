using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using fluXis.Map.Structures.Bases;
using fluXis.Screens.Edit.Tabs.Charting.Playfield;
using fluXis.Utils.Attributes;
using Midori.Utils.Extensions;
using Newtonsoft.Json;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Map.Structures;

[Description("Changes the BPM and time signature.")]
public class TimingPoint : ITimedObject
{
    [JsonProperty("time")]
    [CustomCreateMethod(typeof(ITimedObject), nameof(ITimedObject.CreateVariableTime))]
    public double Time { get; set; }

    [JsonProperty("lane")]
    public int Lane { get; set; }

    [JsonProperty("bpm")]
    public float BPM { get; set; } = 120;

    [JsonProperty("signature")]
    public int Signature { get; set; } = 4;

    [JsonProperty("hide-lines")]
    public bool HideLines { get; set; }

    [JsonIgnore]
    public float MsPerBeat => 60000f / BPM;

    [JsonIgnore]
    string ITimedObject.Group { get; set; }

    IEnumerable<Drawable> ITimedObject.CreateObjectOverlay(EditorDrawableObject obj)
    {
        var flow = (FillFlowContainer)ITimedObject.CreateDefaultOverlay(obj).First();
        flow.Add(ITimedObject.CreateSmallText(obj, () => $"{BPM.ToStringInvariant("0.0")}bpm {Signature}/4"));
        yield return flow;
    }

    public override string ToString() => $"Time: {Time}, BPM: {BPM}, Signature: {Signature}, HideLines: {HideLines}";
}
