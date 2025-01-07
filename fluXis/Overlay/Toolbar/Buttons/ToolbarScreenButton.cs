using System;
using fluXis.Screens;
using osu.Framework.Allocation;
using osu.Framework.Screens;

namespace fluXis.Overlay.Toolbar.Buttons;

public partial class ToolbarScreenButton : ToolbarButton
{
    public Type Screen { get; init; }

    [Resolved]
    private FluXisScreenStack screenStack { get; set; }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        screenStack.ScreenPushed += onScreenChange;
        screenStack.ScreenExited += onScreenChange;
    }

    private void onScreenChange(IScreen lastScreen, IScreen newScreen) => SetLineState(newScreen?.GetType() == Screen);
}
