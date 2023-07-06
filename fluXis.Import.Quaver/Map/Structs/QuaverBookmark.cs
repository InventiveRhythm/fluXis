using JetBrains.Annotations;

namespace fluXis.Import.Quaver.Map.Structs;

[UsedImplicitly(ImplicitUseTargetFlags.Members)]
public class QuaverBookmark
{
    public float StartTime { get; set; }
    public string Note { get; set; }
}
