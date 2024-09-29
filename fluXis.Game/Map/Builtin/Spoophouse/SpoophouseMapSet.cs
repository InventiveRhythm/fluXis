using System.Collections.Generic;
using fluXis.Game.Database.Maps;
using Realms;

namespace fluXis.Game.Map.Builtin.Spoophouse;

[Ignored]
public class SpoophouseMapSet : RealmMapSet
{
    public SpoophouseMapSet()
        : base(new List<RealmMap> { new SpoophouseMap() })
    {
        ID = default;
    }
}
