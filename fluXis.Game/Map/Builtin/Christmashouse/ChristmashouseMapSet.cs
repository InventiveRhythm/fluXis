using System.Collections.Generic;
using fluXis.Game.Database.Maps;
using Realms;

namespace fluXis.Game.Map.Builtin.Christmashouse;

[Ignored]
public class ChristmashouseMapSet : RealmMapSet
{
    public ChristmashouseMapSet()
        : base(new List<RealmMap> { new ChristmashouseMap() })
    {
        ID = default;
    }
}
