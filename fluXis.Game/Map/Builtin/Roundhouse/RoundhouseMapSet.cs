using System.Collections.Generic;
using fluXis.Game.Database.Maps;
using Realms;

namespace fluXis.Game.Map.Builtin.Roundhouse;

[Ignored]
public class RoundhouseMapSet : RealmMapSet
{
    public RoundhouseMapSet()
        : base(new List<RealmMap> { new RoundhouseMap() })
    {
        ID = default;
    }
}
