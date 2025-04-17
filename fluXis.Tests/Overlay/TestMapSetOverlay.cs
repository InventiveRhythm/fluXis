using fluXis.Overlay.MapSet;
using osu.Framework.Allocation;

namespace fluXis.Tests.Overlay;

public partial class TestMapSetOverlay : FluXisTestScene
{
    [BackgroundDependencyLoader]
    private void load()
    {
        var overlay = new MapSetOverlay();
        Add(overlay);

        AddStep("Show MapSet 11", () => overlay.ShowSet(11));
        AddStep("Show MapSet 1", () => overlay.ShowSet(1));
        AddStep("Hide", overlay.Hide);
    }
}
