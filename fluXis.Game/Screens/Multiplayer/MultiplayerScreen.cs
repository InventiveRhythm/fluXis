using fluXis.Game.Screens.Multiplayer.SubScreens;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Screens;

namespace fluXis.Game.Screens.Multiplayer;

public partial class MultiplayerScreen : FluXisScreen
{
    public override float Zoom => 1.1f;
    public override float ParallaxStrength => 2f;
    public override float BackgroundDim => .5f;
    public override float BackgroundBlur => .2f;
    public override bool AllowMusicControl => false;
    public override bool AllowExit => canExit();

    private ScreenStack screenStack;

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChildren = new Drawable[]
        {
            screenStack = new ScreenStack { RelativeSizeAxes = Axes.Both }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        screenStack.Push(new MultiModeSelect());

        screenStack.ScreenExited += (_, _) =>
        {
            if (screenStack.CurrentScreen == null)
                this.Exit();
        };
    }

    private bool canExit()
    {
        while (screenStack.CurrentScreen != null && screenStack.CurrentScreen is not MultiModeSelect)
        {
            var subScreen = (MultiSubScreen)screenStack.CurrentScreen;
            if (subScreen.IsLoaded && subScreen.OnExiting(null))
                return false;

            subScreen.Exit();
        }

        return true;
    }

    public override bool OnExiting(ScreenExitEvent e)
    {
        return !canExit();
    }
}
