using fluXis.Game.Overlay.Profile;

namespace fluXis.Game.Tests.Overlay;

public partial class TestProfileOverlay : FluXisTestScene
{
    public TestProfileOverlay()
    {
        var profileOverlay = new ProfileOverlay();
        Add(profileOverlay);

        AddStep("Set User 1", () => profileOverlay.UpdateUser(1));
        AddStep("Set User 2", () => profileOverlay.UpdateUser(2));
        AddStep("Set User 21", () => profileOverlay.UpdateUser(21));

        AddStep("Show", profileOverlay.Show);
        AddStep("Hide", profileOverlay.Hide);
    }
}
