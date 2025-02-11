using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Database.Maps;
using fluXis.Mods;
using fluXis.Screens.Select;
using osu.Framework.Screens;

namespace fluXis.Screens.Multiplayer.SubScreens.Open.Lobby;

public partial class MultiSongSelect : SelectScreen
{
    private Action<RealmMap, List<string>> selected { get; }

    public MultiSongSelect(Action<RealmMap, List<string>> selected)
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
        var mods = ModSelector.SelectedMods.ToList();
        mods.RemoveAll(x => x is AutoPlayMod);
        selected?.Invoke(map, mods.Select(x => x.Acronym).ToList());
        this.Exit();
    }
}
