using System.Collections.Generic;
using fluXis.Database.Maps;
using Realms;

namespace fluXis.Map.Builtin.Floorboard;

[Ignored]
public class FloorboardMapSet : RealmMapSet
{
    public FloorboardMapSet()
        : base(new List<RealmMap> { new FloorboardMap() })
    {
        ID = default;
    }
}
