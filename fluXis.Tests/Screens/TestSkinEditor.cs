using fluXis.Graphics.Background;
using fluXis.Screens;
using fluXis.Screens.Skinning;
using osu.Framework.Allocation;
using osu.Framework.Graphics;

namespace fluXis.Tests.Screens;

public partial class TestSkinEditor : FluXisTestScene
{
    [BackgroundDependencyLoader]
    private void load()
    {
        CreateClock();

        var background = new GlobalBackground();
        TestDependencies.Cache(background);

        var stack = new FluXisScreenStack();

        AddRange(new Drawable[]
        {
            background,
            stack
        });

        AddStep("Push Skin Editor", () => stack.Push(new SkinEditor()));
    }
}
