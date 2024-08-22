using System.Linq;
using fluXis.Game.Database.Maps;
using fluXis.Game.Graphics.Background;
using fluXis.Game.Graphics.UserInterface.Panel;
using fluXis.Game.Map;
using fluXis.Game.Overlay.Settings;
using fluXis.Game.Screens.Select;
using fluXis.Game.Screens.Select.List;
using fluXis.Game.Screens.Select.List.Items;
using fluXis.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Bindables;

namespace fluXis.Game.Tests.Select;

public partial class TestMapList : FluXisTestScene
{
    [BackgroundDependencyLoader]
    private void load(MapStore store)
    {
        CreateClock();

        var background = new GlobalBackground();
        TestDependencies.Cache(background);

        var panels = new PanelContainer();
        TestDependencies.Cache(panels);

        var settings = new SettingsMenu();
        TestDependencies.Cache(settings);

        var screen = new SelectScreen();
        LoadComponent(screen);
        TestDependencies.Cache(screen);

        var list = new MapList(new Bindable<MapUtils.SortingMode>());
        Add(list);
        list.Show();

        AddStep("Add Item", () =>
        {
            var map = RealmMap.CreateNew();
            list.Insert(new MapSetItem(map.MapSet));
        });

        AddStep("Add All Loaded", () =>
        {
            foreach (var set in store.MapSets)
                list.Insert(new MapSetItem(set));
        });

        AddStep("Clear", () =>
        {
            var items = list.Items.ToList();

            foreach (var item in items) list.Remove(item);
        });
    }
}
