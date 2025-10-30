using System.Linq;
using fluXis.Screens.Select;
using fluXis.Screens.Select.List.Items;
using NUnit.Framework;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Tests.Select;

public partial class TestMapEntry : FluXisTestScene
{
    private ISelectionManager manager;

    [BackgroundDependencyLoader]
    private void load()
    {
        TestDependencies.CacheAs(manager = new DummySelectionManager());
    }

    [Test]
    public void TestSingular() => AddStep("add entry", () =>
    {
        var item = new MapSetItem(CreateDummySet()) { Selection = manager };
        item.Bind();

        var draw = item.CreateDrawable();
        draw.Width = 0.7f;
        draw.Anchor = Anchor.Centre;
        draw.Origin = Anchor.Centre;
        Child = draw;
    });

    [Test]
    public void TestMultiple() => AddStep("add entries", () => Child = new FillFlowContainer
    {
        RelativeSizeAxes = Axes.X,
        AutoSizeAxes = Axes.Y,
        Width = 0.7f,
        Direction = FillDirection.Vertical,
        Spacing = new Vector2(10),
        Anchor = Anchor.Centre,
        Origin = Anchor.Centre,
        ChildrenEnumerable = Enumerable.Range(0, 4).Select(_ =>
        {
            var item = new MapSetItem(CreateDummySet()) { Selection = manager };
            item.Bind();
            return item.CreateDrawable();
        })
    });
}
