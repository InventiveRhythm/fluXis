using System;
using System.Linq;
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

    protected override void Accept()
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

    protected override void OnMapsLoaded() => UpdateSearch();

    protected override void UpdateSearch()
    {
        Maps.Clear();

        foreach (var child in MapList.Content.Children)
        {
            bool matches = child.MapSet.Maps.Aggregate(false, (current, map) => current | Filters.Matches(map));

            if (matches && child.MapSet.OnlineID != -1)
            {
                Maps.Add(child.MapSet);
                child.Show();
            }
            else
                child.Hide();
        }

        /*if (!Maps.Any())
            noMapsContainer.FadeIn(200);
        else
            noMapsContainer.FadeOut(200);*/
    }
}
