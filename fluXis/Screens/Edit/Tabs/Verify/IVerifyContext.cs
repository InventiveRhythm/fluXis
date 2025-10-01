using fluXis.Database.Maps;
using fluXis.Map;
using fluXis.Online.API.Models.Maps;
using osu.Framework.Graphics;

namespace fluXis.Screens.Edit.Tabs.Verify;

public interface IVerifyContext
{
    MapInfo MapInfo { get; }
    MapEvents MapEvents { get; }
    RealmMap RealmMap { get; }

    int MaxKeyCount => RealmMap.KeyCount * (MapInfo.DualMode == DualMode.Separate ? 2 : 1);
    RealmMapSet MapSet => RealmMap.MapSet;

    void LoadComponent(Drawable drawable);
}
