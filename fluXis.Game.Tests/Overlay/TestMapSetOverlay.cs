using fluXis.Game.Overlay.MapSet;
using osu.Framework.Allocation;

namespace fluXis.Game.Tests.Overlay;

public partial class TestMapSetOverlay : FluXisTestScene
{
    [BackgroundDependencyLoader]
    private void load()
    {
        var overlay = new MapSetOverlay();
        Add(overlay);

        AddStep("Show MapSet 1", () => overlay.ShowSet(1));
        AddStep("Show MapSet 71", () => overlay.ShowSet(71));
        AddStep("Hide", overlay.Hide);
    }
}
