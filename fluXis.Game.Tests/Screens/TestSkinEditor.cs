using fluXis.Game.Screens;
using fluXis.Game.Screens.Skin;

namespace fluXis.Game.Tests.Screens;

public partial class TestSkinEditor : FluXisTestScene
{
    public TestSkinEditor()
    {
        var stack = new FluXisScreenStack();
        Add(stack);

        AddStep("Push Skin Editor", () => stack.Push(new SkinEditor()));
    }
}
