using fluXis.Game.Map.Structures.Bases;
using Newtonsoft.Json;

namespace fluXis.Game.Map.Structures;

public class TimingPoint : ITimedObject
{
    [JsonProperty("time")]
    public double Time { get; set; }

    [JsonProperty("bpm")]
    public float BPM { get; set; }

    [JsonProperty("signature")]
    public int Signature { get; set; }

    [JsonProperty("hide-lines")]
    public bool HideLines { get; set; }

    [JsonIgnore]
    public float MsPerBeat => 60000f / BPM;

    public TimingPoint Copy()
    {
        return new TimingPoint
        {
            Time = Time,
            BPM = BPM,
            Signature = Signature,
            HideLines = HideLines
        };
    }

    public override string ToString() => $"Time: {Time}, BPM: {BPM}, Signature: {Signature}, HideLines: {HideLines}";
}
