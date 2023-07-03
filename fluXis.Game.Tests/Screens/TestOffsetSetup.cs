using fluXis.Game.Graphics;
using fluXis.Game.Screens;
using fluXis.Game.Screens.Offset;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Game.Tests.Screens;

public partial class TestOffsetSetup : FluXisTestScene
{
    public TestOffsetSetup()
    {
        var stack = new FluXisScreenStack();

        Children = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = FluXisColors.Background2
            },
            stack
        };

        AddStep("Push OffsetSetup", () => stack.Push(new OffsetSetup()));
    }
}
