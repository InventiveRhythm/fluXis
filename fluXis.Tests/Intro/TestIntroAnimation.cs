using fluXis.Screens;
using fluXis.Screens.Intro;

namespace fluXis.Tests.Intro;

public partial class TestIntroAnimation : FluXisTestScene
{
    public TestIntroAnimation()
    {
        var stack = new FluXisScreenStack();
        stack.Push(new IntroAnimation());
    }
}
