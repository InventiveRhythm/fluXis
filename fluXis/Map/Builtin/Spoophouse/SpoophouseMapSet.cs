using System.Collections.Generic;
using fluXis.Database.Maps;
using Realms;

namespace fluXis.Map.Builtin.Spoophouse;

[Ignored]
public class SpoophouseMapSet : RealmMapSet
{
    public SpoophouseMapSet()
        : base(new List<RealmMap> { new SpoophouseMap() })
    {
        ID = default;
    }
}
