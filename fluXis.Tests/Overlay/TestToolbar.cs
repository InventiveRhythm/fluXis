using fluXis.Graphics.UserInterface.Panel;
using fluXis.Overlay.Chat;
using fluXis.Overlay.Music;
using fluXis.Overlay.Network;
using fluXis.Overlay.Settings;
using fluXis.Overlay.Toolbar;
using fluXis.Overlay.User;
using fluXis.Screens;
using osu.Framework.Allocation;

namespace fluXis.Tests.Overlay;

public partial class TestToolbar : FluXisTestScene
{
    [BackgroundDependencyLoader]
    private void load()
    {
        CreateClock();

        TestDependencies.Cache(new SettingsMenu());
        TestDependencies.Cache(new MusicPlayer());
        TestDependencies.Cache(new Dashboard());
        TestDependencies.Cache(new ChatOverlay());
        TestDependencies.Cache(new FluXisScreenStack());
        TestDependencies.Cache(new UserProfileOverlay());

        var panels = new PanelContainer();
        TestDependencies.Cache(panels);

        var toolbar = new Toolbar();
        Add(toolbar);

        Add(panels);

        AddStep("Toggle Toolbar", toolbar.ToggleVisibility);
    }
}
