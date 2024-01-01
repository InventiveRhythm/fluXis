using System;
using fluXis.Game.Database.Maps;

namespace fluXis.Game.Screens.Gameplay.Replay;

public partial class ReplayLoader : GameplayLoader
{
    public ReplayLoader(RealmMap map, Func<GameplayScreen> create)
        : base(map, create)
    {
    }
}
