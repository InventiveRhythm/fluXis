using YamlDotNet.Serialization;

namespace fluXis.Game.Import.Quaver.Map.Structs;

public class QuaverHitObjectInfo
{
    public float StartTime { get; set; }
    public int Lane { get; set; }
    public float EndTime { get; set; }

    [YamlIgnore]
    public bool IsLongNote => EndTime > 0;
}
