using fluXis.Game.Graphics.UserInterface.Panel;
using fluXis.Game.Overlay.Auth;
using fluXis.Game.Overlay.Chat;
using fluXis.Game.Overlay.Music;
using fluXis.Game.Overlay.Network;
using fluXis.Game.Overlay.Settings;
using fluXis.Game.Overlay.Toolbar;
using fluXis.Game.Overlay.User;
using fluXis.Game.Screens;
using osu.Framework.Allocation;

namespace fluXis.Game.Tests.Overlay;

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
        TestDependencies.Cache(new LoginOverlay());

        var panels = new PanelContainer();
        TestDependencies.Cache(panels);

        var toolbar = new Toolbar();
        Add(toolbar);

        Add(panels);

        AddStep("Toggle Toolbar", toolbar.ShowToolbar.Toggle);
    }
}
