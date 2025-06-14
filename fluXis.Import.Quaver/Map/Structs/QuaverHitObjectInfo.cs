using JetBrains.Annotations;
using YamlDotNet.Serialization;

namespace fluXis.Import.Quaver.Map.Structs;

[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public class QuaverHitObjectInfo
{
    public float StartTime { get; set; }
    public int Lane { get; set; }
    public float EndTime { get; set; }
    public string TimingGroup { get; set; }

    [YamlIgnore]
    public bool IsLongNote => EndTime > 0;
}
