using fluXis.Game.Graphics.Background;
using fluXis.Game.Graphics.UserInterface.Panel;
using fluXis.Game.Online.Fluxel;
using fluXis.Game.Screens.Multiplayer;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Screens;

namespace fluXis.Game.Tests.Multiplayer;

public partial class TestMultiplayer : FluXisTestScene
{
    [Resolved]
    private IAPIClient api { get; set; }

    [BackgroundDependencyLoader]
    private void load()
    {
        CreateClock();

        var backgrounds = new GlobalBackground();
        TestDependencies.CacheAs(backgrounds);

        var panels = new PanelContainer();
        TestDependencies.CacheAs(panels);

        var stack = new ScreenStack { RelativeSizeAxes = Axes.Both };

        Children = new Drawable[]
        {
            backgrounds,
            stack,
            panels
        };

        AddStep("Push MultiplayerScreen", () => stack.Push(new MultiplayerScreen()));
    }
}
