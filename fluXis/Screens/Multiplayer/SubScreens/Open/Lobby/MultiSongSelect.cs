using System;
using fluXis.Database.Maps;
using fluXis.Screens.Select;
using osu.Framework.Screens;

namespace fluXis.Screens.Multiplayer.SubScreens.Open.Lobby;

public partial class MultiSongSelect : SelectScreen
{
    private Action<RealmMap> selected { get; }

    public MultiSongSelect(Action<RealmMap> selected)
    {
        this.selected = selected;
    }

    protected override bool ShouldAdd(RealmMapSet set) => set.OnlineID > 0 && set.Maps[0].StatusInt > (int)MapStatus.Local;

    public override void Accept()
    {
        if (MapStore.CurrentMap == null)
            return;

        // just in case, who knows
        if (MapStore.CurrentMap.OnlineID < 1)
            return;

        var map = MapStore.CurrentMap;
        selected?.Invoke(map);
        this.Exit();
    }
}
