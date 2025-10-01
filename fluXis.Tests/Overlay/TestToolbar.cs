using fluXis.Overlay.Browse;
using fluXis.Overlay.Chat;
using fluXis.Overlay.Music;
using fluXis.Overlay.Network;
using fluXis.Overlay.Settings;
using fluXis.Overlay.Toolbar;
using fluXis.Overlay.User;
using fluXis.Overlay.Wiki;
using fluXis.Screens;
using NUnit.Framework;

namespace fluXis.Tests.Overlay;

public partial class TestToolbar : FluXisTestScene
{
    protected override bool UseTestAPI => true;

    private Toolbar toolbar;
    private bool cached = false;

    [SetUp]
    public void Setup() => Schedule(() =>
    {
        Clear();

        if (!cached)
        {
            CreateClock();

            TestDependencies.Cache(new SettingsMenu());
            TestDependencies.Cache(new FluXisScreenStack());

            TestDependencies.Cache(new ChatOverlay());
            TestDependencies.Cache(new Dashboard());
            TestDependencies.Cache(new BrowseOverlay());
            TestDependencies.Cache(new WikiOverlay());
            TestDependencies.Cache(new MusicPlayer());
            TestDependencies.Cache(new UserProfileOverlay());
        }

        cached = true;
        Add(toolbar = new Toolbar());
    });

    [Test]
    public void TestBase() => show();

    [Test]
    public void TestVisibility()
    {
        show();
        wait();
        hide();
    }

    private void show() => AddStep("Show Toolbar", () => toolbar.Show());
    private void hide() => AddStep("Hide Toolbar", () => toolbar.Hide());
    private void wait() => AddWaitStep("wait", 3);
}
