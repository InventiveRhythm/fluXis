using System;
using fluXis.Game.Database.Maps;
using fluXis.Game.Screens.Select;
using osu.Framework.Screens;

namespace fluXis.Game.Screens.Multiplayer.SubScreens.Open.Lobby;

public partial class MultiSongSelect : SelectScreen
{
    private Action<RealmMap> selected { get; }

    public MultiSongSelect(Action<RealmMap> selected)
    {
        this.selected = selected;
    }

    protected override bool ShouldAdd(RealmMapSet set) => set.OnlineID > 0;

    public override void Accept()
    {
        if (MapStore.CurrentMap == null)
            return;

        // just in case, who knows
        if (MapStore.CurrentMap.OnlineID == -1)
            return;

        var map = MapStore.CurrentMap;
        selected?.Invoke(map);
        this.Exit();
    }
}
