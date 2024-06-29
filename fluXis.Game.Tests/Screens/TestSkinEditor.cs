using fluXis.Game.Graphics.Background;
using fluXis.Game.Screens;
using fluXis.Game.Screens.Skinning;
using osu.Framework.Allocation;
using osu.Framework.Graphics;

namespace fluXis.Game.Tests.Screens;

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
