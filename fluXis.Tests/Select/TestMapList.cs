using fluXis.Graphics.Background;
using fluXis.Graphics.UserInterface.Panel;
using fluXis.Overlay.Settings;
using fluXis.Screens.Select;
using fluXis.Screens.Select.List;
using fluXis.Screens.Select.List.Items;
using fluXis.Utils;
using NUnit.Framework;
using osu.Framework.Allocation;
using osu.Framework.Bindables;

namespace fluXis.Tests.Select;

public partial class TestMapList : FluXisTestScene
{
    private MapList list;

    [BackgroundDependencyLoader]
    private void load()
    {
        CreateClock();

        TestDependencies.Cache(new GlobalBackground());
        TestDependencies.Cache(new PanelContainer());
        TestDependencies.Cache(new SettingsMenu());

        var screen = new SoloSelectScreen();
        TestDependencies.CacheAs<SelectScreen>(screen);
        LoadComponent(screen);

        Add(list = new MapList(new Bindable<MapUtils.SortingMode>()));
        TestDependencies.CacheAs<ISelectionManager>(list);
        list.Show();
    }

    [SetUp]
    public void Setup()
    {
        list?.Clear();
    }

    [Test]
    public void TestSingleSet()
    {
        AddStep("Add Item", () =>
        {
            var set = CreateDummySet();
            list.Insert(new MapSetItem(set));
        });
    }

    [Test]
    public void TestSorting()
    {
        createMapsCount(25);
        AddStep("Change Sorting to Title", () => list.SetSorting(MapUtils.SortingMode.Title));
        AddStep("Change Sorting to Difficulty", () => list.SetSorting(MapUtils.SortingMode.Difficulty));
    }

    [Test]
    public void TestManyMaps()
    {
        createMapsCount(20000);
    }

    private void createMapsCount(int count)
    {
        AddStep($"Add {count} maps", () =>
        {
            list.StartBulkInsert();

            for (int i = 0; i < count; i++)
            {
                var set = CreateDummySet();
                list.Insert(new MapSetItem(set));
            }

            list.EndBulkInsert();
        });
    }
}
