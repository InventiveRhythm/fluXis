using fluXis.Game.Graphics.Background;
using fluXis.Game.Screens.Edit;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Screens;

namespace fluXis.Game.Tests.Edit;

public partial class TestEditor : FluXisTestScene
{
    [Resolved]
    private BackgroundStack backgroundStack { get; set; }

    private ScreenStack screenStack { get; set; }
    private Editor editor { get; set; }

    public TestEditor()
    {
        screenStack = new ScreenStack
        {
            RelativeSizeAxes = Axes.Both
        };
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        Add(backgroundStack);
        Add(screenStack);

        editor = new Editor();
        screenStack.Push(editor);
    }

    protected override void LoadComplete()
    {
        editor.ChangeTab(1);
        base.LoadComplete();
    }
}
