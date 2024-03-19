using fluXis.Game.Graphics.Background;
using fluXis.Game.Graphics.UserInterface.Panel;
using fluXis.Game.Map;
using fluXis.Game.Overlay.Settings;
using fluXis.Game.Screens;
using fluXis.Game.Screens.Multiplayer.SubScreens.Open.Lobby;
using osu.Framework.Allocation;
using osu.Framework.Graphics;

namespace fluXis.Game.Tests.Multiplayer;

public partial class TestMultiSongSelect : FluXisTestScene
{
    [BackgroundDependencyLoader]
    private void load(MapStore store)
    {
        CreateClock();

        var background = new GlobalBackground();
        TestDependencies.Cache(background);

        var stack = new FluXisScreenStack();
        TestDependencies.Cache(stack);

        var settings = new SettingsMenu();
        TestDependencies.Cache(settings);

        var panels = new PanelContainer();
        TestDependencies.Cache(panels);

        AddRange(new Drawable[]
        {
            GlobalClock,
            background,
            stack,
            panels
        });

        store.CurrentMap = store.GetRandom()?.LowestDifficulty;

        AddStep("Push SongSelect", () => stack.Push(new MultiSongSelect(null)));
    }
}
