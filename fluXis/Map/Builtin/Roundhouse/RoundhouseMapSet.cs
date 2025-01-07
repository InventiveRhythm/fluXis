using System.Collections.Generic;
using fluXis.Database.Maps;
using Realms;

namespace fluXis.Map.Builtin.Roundhouse;

[Ignored]
public class RoundhouseMapSet : RealmMapSet
{
    public RoundhouseMapSet()
        : base(new List<RealmMap> { new RoundhouseMap() })
    {
        ID = default;
    }
}
