using fluXis.Database.Maps;
using fluXis.Map;
using fluXis.Online.API.Models.Maps;

namespace fluXis.Screens.Edit.Tabs.Verify;

public interface IVerifyContext
{
    MapInfo MapInfo { get; }
    MapEvents MapEvents { get; }
    RealmMap RealmMap { get; }

    public int MaxKeyCount => RealmMap.KeyCount * (MapInfo.DualMode == DualMode.Separate ? 2 : 1);
    public RealmMapSet MapSet => RealmMap.MapSet;
}
