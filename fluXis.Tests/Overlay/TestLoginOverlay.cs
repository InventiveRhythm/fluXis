using fluXis.Overlay.Auth;
using NUnit.Framework;
using osu.Framework.Allocation;

namespace fluXis.Tests.Overlay;

public partial class TestLoginOverlay : FluXisTestScene
{
    protected override bool UseTestAPI => true;

    private LoginOverlay overlay;

    [BackgroundDependencyLoader]
    private void load()
    {
        CreateDummyBeatSync();
    }

    [SetUp]
    public void Setup() => Schedule(() =>
    {
        Child = overlay = new LoginOverlay();
    });

    [Test]
    public void TestAnimations()
    {
        AddStep("logout", () => TestAPI.Logout());
        AddStep("show overlay", () => overlay.Show());
        AddRepeatStep("wait", () => { }, 4);
        AddAssert("check overlay is visible", () => overlay.Alpha == 1);
        AddStep("hide overlay", () => overlay.Hide());
        AddRepeatStep("wait", () => { }, 4);
        AddAssert("check overlay is hidden", () => overlay.Alpha == 0);
    }
}
