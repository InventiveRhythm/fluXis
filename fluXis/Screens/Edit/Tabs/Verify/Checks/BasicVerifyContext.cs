using System;
using fluXis.Database.Maps;
using fluXis.Map;

namespace fluXis.Screens.Edit.Tabs.Verify.Checks;

public class BasicVerifyContext : IVerifyContext
{
    public MapInfo MapInfo { get; }
    public MapEvents MapEvents { get; }
    public RealmMap RealmMap { get; }

    public BasicVerifyContext(RealmMap map)
    {
        RealmMap = map;
        MapInfo = map.GetMapInfo() ?? throw new InvalidOperationException($"Could not load map file from {map.FileName}!");
        MapEvents = MapInfo.GetMapEvents();
    }
}
