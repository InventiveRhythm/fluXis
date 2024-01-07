using fluXis.Game.Overlay.User;
using osu.Framework.Allocation;

namespace fluXis.Game.Tests.Overlay;

public partial class TestUserProfileOverlay : FluXisTestScene
{
    [BackgroundDependencyLoader]
    private void load()
    {
        var overlay = new UserProfileOverlay();
        Add(overlay);

        AddStep("Show User 1", () => overlay.ShowUser(1));
        AddStep("Show User 2", () => overlay.ShowUser(2));
        AddStep("Hide", overlay.Hide);
    }
}
