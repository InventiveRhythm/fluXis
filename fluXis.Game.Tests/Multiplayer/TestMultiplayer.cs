using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Online.Fluxel;
using fluXis.Game.Screens.Multiplayer;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Screens;

namespace fluXis.Game.Tests.Multiplayer;

public partial class TestMultiplayer : FluXisTestScene
{
    [Resolved]
    private FluxelClient fluxel { get; set; }

    [BackgroundDependencyLoader]
    private void load()
    {
        var stack = new ScreenStack { RelativeSizeAxes = Axes.Both };

        Children = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = FluXisColors.Background1
            },
            stack
        };

        AddStep("Push MultiplayerScreen", () => stack.Push(new MultiplayerScreen()));
    }
}
