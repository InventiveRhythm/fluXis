using Newtonsoft.Json;

namespace fluXis.Game.Map;

public class TimingPointInfo : TimedObject
{
    public float BPM;
    public int Signature;
    public bool HideLines;

    [JsonIgnore]
    public float MsPerBeat => 60000f / BPM;

    public override string ToString() => $"Time: {Time}, BPM: {BPM}, Signature: {Signature}, HideLines: {HideLines}";
}
