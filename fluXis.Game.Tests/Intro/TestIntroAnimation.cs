using fluXis.Game.Screens;
using fluXis.Game.Screens.Intro;

namespace fluXis.Game.Tests.Intro;

public partial class TestIntroAnimation : FluXisTestScene
{
    public TestIntroAnimation()
    {
        var stack = new FluXisScreenStack();
        stack.Push(new IntroAnimation());
    }
}
