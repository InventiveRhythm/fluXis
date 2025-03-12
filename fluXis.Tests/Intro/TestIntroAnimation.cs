using fluXis.Graphics.Background;
using fluXis.Screens;
using fluXis.Screens.Intro;
using NUnit.Framework;

namespace fluXis.Tests.Intro;

public partial class TestIntroAnimation : FluXisTestScene
{
    private FluXisScreenStack screens;
    private bool cached;

    [SetUp]
    public void Setup() => Schedule(() =>
    {
        if (!cached)
        {
            TestDependencies.CacheAs(new GlobalBackground());
            cached = true;
        }

        screens = new FluXisScreenStack();
        Add(screens);
    });

    [Test]
    public void TestIntro()
    {
        AddStep("Push Screen", () => screens.Push(new IntroAnimation { TargetScreen = new EmptyScreen() }));
    }

    private partial class EmptyScreen : FluXisScreen
    {
    }
}
