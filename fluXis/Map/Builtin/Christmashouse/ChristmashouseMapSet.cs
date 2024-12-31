using System.Collections.Generic;
using fluXis.Database.Maps;
using Realms;

namespace fluXis.Map.Builtin.Christmashouse;

[Ignored]
public class ChristmashouseMapSet : RealmMapSet
{
    public ChristmashouseMapSet()
        : base(new List<RealmMap> { new ChristmashouseMap() })
    {
        ID = default;
    }
}
