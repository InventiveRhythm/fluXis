using System;
using System.Collections.Generic;
using System.Linq;
using fluXis.Database.Maps;
using fluXis.Mods;
using fluXis.Scoring;
using fluXis.Screens.Select;
using osu.Framework.Screens;

namespace fluXis.Screens.Multiplayer.SubScreens.Open.Lobby;

public partial class MultiSelectScreen : SelectScreen
{
    public override bool PlayBackSound => !hasSelected;
    private bool hasSelected;

    private Action<RealmMap, List<string>> selected { get; }

    public MultiSelectScreen(Action<RealmMap, List<string>> selected)
    {
        this.selected = selected;
    }

    protected override bool ShouldAdd(RealmMapSet set) => set.OnlineID > 0 && set.Maps[0].StatusInt > (int)MapStatus.Local;

    protected override void StartMap(RealmMap map, List<IMod> mods, List<ScoreInfo> scores)
    {
        if (map == null)
            return;

        // just in case, who knows
        if (map.OnlineID < 1)
            return;

        mods.RemoveAll(x => x is AutoPlayMod);
        selected?.Invoke(map, mods.Select(x => x.Acronym).ToList());
        hasSelected = true;
        this.Exit();
    }
}
