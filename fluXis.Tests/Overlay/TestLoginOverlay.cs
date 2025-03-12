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
    public void TestShow()
    {
        AddStep("logout", () => TestAPI.Logout());
        AddStep("show overlay", () => overlay.Show());
        AddRepeatStep("wait", () => { }, 4);
        AddAssert("check overlay is visible", () => overlay.Alpha == 1);
    }

    [Test]
    public void TestHide()
    {
        AddStep("logout", () => TestAPI.Logout());
        AddStep("show overlay", () => overlay.Show());
        AddRepeatStep("wait", () => { }, 4);
        AddStep("hide overlay", () => overlay.Hide());
        AddRepeatStep("wait", () => { }, 4);
        AddAssert("check overlay is hidden", () => overlay.Alpha == 0);
    }

    [Test]
    public void TestDontShowWhenLoggedIn()
    {
        AddStep("log in", () => TestAPI.Login(TestAPIClient.USERNAME, TestAPIClient.PASSWORD));
        AddStep("show overlay", () => overlay.Show());
        AddRepeatStep("wait", () => { }, 4);
        AddAssert("check overlay is hidden", () => overlay.Alpha == 0);
    }
}
