using System.ComponentModel;
using fluXis.Map.Structures.Bases;
using fluXis.Utils.Attributes;
using Newtonsoft.Json;

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

    public override string ToString() => $"Time: {Time}, BPM: {BPM}, Signature: {Signature}, HideLines: {HideLines}";
}
