using fluXis.Game.Graphics.Background;
using fluXis.Game.Screens;
using fluXis.Game.Screens.Import;
using osu.Framework.Allocation;

namespace fluXis.Game.Tests.Screens;

public partial class TestFileImportScreen : FluXisTestScene
{
    [Cached]
    private BackgroundStack backgrounds;

    private readonly FluXisScreenStack screenStack;

    public TestFileImportScreen()
    {
        backgrounds = new BackgroundStack();
        Add(backgrounds);
        Add(screenStack = new FluXisScreenStack());
    }

    protected override void LoadComplete()
    {
        backgrounds.AddBackgroundFromMap(null);
        AddStep("Push FileImportScreen", () => screenStack.Push(new FileImportScreen()));
        AddStep("Exit FileImportScreen", () => screenStack.Exit());
    }
}
