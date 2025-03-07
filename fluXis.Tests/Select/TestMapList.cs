using fluXis.Graphics.Background;
using fluXis.Graphics.UserInterface.Panel;
using fluXis.Overlay.Settings;
using fluXis.Screens.Select;
using fluXis.Screens.Select.List;
using fluXis.Screens.Select.List.Items;
using fluXis.Utils;
using NUnit.Framework;
using osu.Framework.Bindables;

namespace fluXis.Tests.Select;

public partial class TestMapList : FluXisTestScene
{
    private bool cached = false;
    private MapList list;

    [SetUp]
    public void Setup()
    {
        Clear();

        if (!cached)
        {
            CreateClock();

            TestDependencies.Cache(new GlobalBackground());
            TestDependencies.Cache(new PanelContainer());
            TestDependencies.Cache(new SettingsMenu());

            var screen = new SoloSelectScreen();
            TestDependencies.Cache(screen);
            LoadComponent(screen);
        }

        cached = true;

        list = new MapList(new Bindable<MapUtils.SortingMode>());
        Add(list);
        list.Show();
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
    public void TestManyMaps()
    {
        createMapsCount(80000);
    }

    [Test]
    public void TestSorting()
    {
        createMapsCount(25);
        AddStep("Change Sorting to Title", () => list.SetSorting(MapUtils.SortingMode.Title));
        AddStep("Change Sorting to Difficulty", () => list.SetSorting(MapUtils.SortingMode.Difficulty));
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
