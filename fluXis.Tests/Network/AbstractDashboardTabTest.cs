using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Panel;
using fluXis.Overlay.Network;
using NUnit.Framework;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Tests.Network;

public abstract partial class AbstractDashboardTabTest<T> : FluXisTestScene
    where T : DashboardTab, new()
{
    protected override bool UseTestAPI => true;

    private bool cached;

    [SetUp]
    public void Setup()
    {
        TestAPI.TestLogin();

        Schedule(() =>
        {
            Clear();

            if (!cached)
            {
                var panels = new PanelContainer();
                TestDependencies.Cache(panels);

                cached = true;
            }

            Add(new Box { RelativeSizeAxes = Axes.Both, Colour = Theme.Background2 });
        });
    }

    [Test]
    public void TestDefault() => CreateAddStep();

    protected void CreateAddStep()
        => AddStep($"Add {typeof(T).Name}", () => AddTab());

    protected T AddTab()
    {
        var tab = new T();
        tab.RelativeSizeAxes = Axes.Both;
        tab.OnLoadComplete += t =>
        {
            tab.Width = 1;
            tab.Enter();
        };
        Add(tab);
        return tab;
    }
}
