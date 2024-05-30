using fluXis.Game.Overlay.Login;
using osu.Framework.Allocation;

namespace fluXis.Game.Tests.Overlay;

public partial class TestLoginOverlay : FluXisTestScene
{
    [BackgroundDependencyLoader]
    private void load()
    {
        CreateDummyBeatSync();

        var overlay = new LoginOverlay();
        Add(overlay);

        AddStep("Show", () => overlay.Show());
    }
}
