using Newtonsoft.Json;

namespace fluXis.Game.Map.Structures;

public class TimingPoint : TimedObject
{
    public float BPM { get; set; }
    public int Signature { get; set; }
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
