using JetBrains.Annotations;

namespace fluXis.Import.Quaver.Map.Structs;

[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public class QuaverTimingPointInfo
{
    public float StartTime { get; set; }
    public float Bpm { get; set; }
    public int TimeSignature { get; set; }
    public bool Hidden { get; set; }
}
