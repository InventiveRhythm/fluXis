using fluXis.Overlay.Club;
using osu.Framework.Allocation;

namespace fluXis.Tests.Overlay;

public partial class TestClubOverlay : FluXisTestScene
{
    [BackgroundDependencyLoader]
    private void load()
    {
        var overlay = new ClubOverlay();
        Add(overlay);

        AddStep("Show Club 1", () => overlay.ShowClub(1));
        AddStep("Show Club 2", () => overlay.ShowClub(2));
        AddStep("Hide", overlay.Hide);
    }
}
