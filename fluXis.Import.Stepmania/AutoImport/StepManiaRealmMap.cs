using fluXis.Database.Maps;
using fluXis.Map;
using fluXis.Modes;

namespace fluXis.Import.Stepmania.AutoImport;

public class StepManiaRealmMap : RealmMap
{
    public PlayableMap PlayableMap { get; init; } = null!;

    public override PlayableMap GetPlayable(GameModeManager modes) => PlayableMap;
}
