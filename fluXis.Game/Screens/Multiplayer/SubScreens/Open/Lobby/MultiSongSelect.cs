using System.Linq;
using fluXis.Game.Screens.Select;

namespace fluXis.Game.Screens.Multiplayer.SubScreens.Open.Lobby;

public partial class MultiSongSelect : SelectScreen
{
    public override void Accept()
    {
        if (MapStore.CurrentMap == null)
            return;

        // just in case, who knows
        if (MapStore.CurrentMap.OnlineID == -1)
            return;

        // add to playlist
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
