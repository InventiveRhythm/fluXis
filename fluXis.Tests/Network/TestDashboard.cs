using fluXis.Graphics.UserInterface.Panel;
using fluXis.Overlay.Network;
using osu.Framework.Allocation;

namespace fluXis.Tests.Network;

public partial class TestDashboard : FluXisTestScene
{
    protected override bool UseTestAPI => true;

    [BackgroundDependencyLoader]
    private void load()
    {
        CreateClock();

        var panels = new PanelContainer();
        TestDependencies.Cache(panels);

        var dash = new Dashboard();
        TestDependencies.Cache(dash);
        Add(dash);

        AddStep("toggle visibility", dash.ToggleVisibility);
    }
}
