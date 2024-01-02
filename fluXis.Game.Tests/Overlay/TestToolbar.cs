using fluXis.Game.Overlay.Toolbar;
using osu.Framework.Allocation;

namespace fluXis.Game.Tests.Overlay;

public partial class TestToolbar : FluXisTestScene
{
    [BackgroundDependencyLoader]
    private void load()
    {
        var toolbar = new Toolbar();
        Add(toolbar);

        AddStep("Toggle Toolbar", toolbar.ShowToolbar.Toggle);
    }
}
