using fluXis.Game.Map.Structures.Bases;
using Newtonsoft.Json;

namespace fluXis.Game.Map.Structures;

public class TimingPoint : ITimedObject
{
    [JsonProperty("time")]
    public double Time { get; set; }

    [JsonProperty("bpm")]
    public float BPM { get; set; } = 120;

    [JsonProperty("signature")]
    public int Signature { get; set; } = 4;

    [JsonProperty("hide-lines")]
    public bool HideLines { get; set; }

    [JsonIgnore]
    public float MsPerBeat => 60000f / BPM;

    public override string ToString() => $"Time: {Time}, BPM: {BPM}, Signature: {Signature}, HideLines: {HideLines}";
}
