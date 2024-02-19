using fluXis.Game.Database.Maps;
using fluXis.Game.Map;

namespace fluXis.Import.Stepmania.AutoImport;

public class StepManiaRealmMap : RealmMap
{
    public MapInfo MapInfo { get; init; } = null!;

    public override MapInfo GetMapInfo() => MapInfo;
}
