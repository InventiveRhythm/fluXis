using fluXis.Overlay.Browse;
using osu.Framework.Allocation;

namespace fluXis.Tests.Overlay;

public partial class TestBrowseOverlay : FluXisTestScene
{
    [BackgroundDependencyLoader]
    private void load()
    {
        CreateDummyBeatSync();

        var overlay = new BrowseOverlay();
        Add(overlay);

        AddStep("Show", overlay.Show);
        AddStep("Hide", overlay.Hide);
    }
}
