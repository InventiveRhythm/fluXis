using fluXis.Graphics.Background;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Screens;
using fluXis.Screens.Offset;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Tests.Screens;

public partial class TestOffsetSetup : FluXisTestScene
{
    [BackgroundDependencyLoader]
    private void load()
    {
        CreateClock();

        var background = new GlobalBackground();
        TestDependencies.Cache(background);
        Add(background);

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
