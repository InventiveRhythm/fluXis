using fluXis.Game.Map;

namespace fluXis.Import.Quaver.Map;

public class QuaverMapInfo : MapInfo
{
    public new QuaverMap Map { get; init; }

    public QuaverMapInfo(MapMetadata metadata)
        : base(metadata)
    {
    }

    public override MapEvents GetMapEvents() => Map.GetEffects();
}
