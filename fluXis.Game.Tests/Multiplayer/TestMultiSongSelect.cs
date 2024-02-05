using fluXis.Game.Audio;
using fluXis.Game.Graphics.Background;
using fluXis.Game.Map;
using fluXis.Game.Screens;
using fluXis.Game.Screens.Multiplayer.SubScreens.Open.Lobby;
using osu.Framework.Allocation;

namespace fluXis.Game.Tests.Multiplayer;

public partial class TestMultiSongSelect : FluXisTestScene
{
    [BackgroundDependencyLoader]
    private void load(MapStore store)
    {
        var clock = new GlobalClock();
        TestDependencies.Cache(clock);
        Add(clock);

        var background = new GlobalBackground();
        TestDependencies.Cache(background);
        Add(background);

        store.CurrentMap = store.GetRandom()?.LowestDifficulty;

        var stack = new FluXisScreenStack();
        Add(stack);

        AddStep("Push SongSelect", () => stack.Push(new MultiSongSelect()));
    }
}
