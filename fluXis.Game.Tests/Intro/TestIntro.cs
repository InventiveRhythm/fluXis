using fluXis.Game.Screens;
using fluXis.Game.Screens.Warning;

namespace fluXis.Game.Tests.Intro;

public partial class TestIntro : FluXisTestScene
{
    public TestIntro()
    {
        var stack = new FluXisScreenStack();
        Add(stack);

        AddStep("Play Intro", () => stack.Push(new WarningScreen()));
    }
}
