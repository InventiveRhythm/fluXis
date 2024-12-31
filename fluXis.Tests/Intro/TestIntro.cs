using fluXis.Screens;
using fluXis.Screens.Warning;

namespace fluXis.Tests.Intro;

public partial class TestIntro : FluXisTestScene
{
    public TestIntro()
    {
        var stack = new FluXisScreenStack();
        Add(stack);

        AddStep("Play Intro", () => stack.Push(new WarningScreen()));
    }
}
