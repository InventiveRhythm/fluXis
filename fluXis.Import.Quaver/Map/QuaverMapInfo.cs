using fluXis.Game.Map;
using Newtonsoft.Json;

namespace fluXis.Import.Quaver.Map;

public class QuaverMapInfo : MapInfo
{
    [JsonIgnore]
    public new QuaverMap Map { get; init; }

    public QuaverMapInfo(MapMetadata metadata)
        : base(metadata)
    {
    }

    public override MapEvents GetMapEvents() => Map.GetEffects();
}
