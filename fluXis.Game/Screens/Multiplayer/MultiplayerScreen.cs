using fluXis.Game.Audio;
using fluXis.Game.Graphics.Background;
using fluXis.Game.Map;
using fluXis.Game.Screens.Multiplayer.SubScreens;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Screens;

namespace fluXis.Game.Screens.Multiplayer;

[Cached]
public partial class MultiplayerScreen : FluXisScreen
{
    public override float Zoom => 1.1f;
    public override float ParallaxStrength => .05f;
    public override float BackgroundDim => .5f;
    public override float BackgroundBlur => .2f;
    public override bool AllowMusicControl => false;
    public override bool AllowExit => canExit();

    [Resolved]
    private GlobalClock globalClock { get; set; }

    [Resolved]
    private MapStore mapStore { get; set; }

    [Resolved]
    private GlobalBackground backgrounds { get; set; }

    [Cached]
    private MultiplayerMenuMusic menuMusic = new();

    private ScreenStack screenStack;

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChildren = new Drawable[]
        {
            menuMusic,
            screenStack = new ScreenStack
            {
                RelativeSizeAxes = Axes.Both,
                Alpha = 0
            }
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

    public override void OnEntering(ScreenTransitionEvent e)
    {
        using (BeginDelayedSequence(ENTER_DELAY))
            screenStack.FadeIn(FADE_DURATION);

        globalClock.VolumeOut(400).OnComplete(c => c.Stop());
        backgrounds.AddBackgroundFromMap(null);
    }

    public override bool OnExiting(ScreenExitEvent e)
    {
        if (!canExit()) return true;

        menuMusic.StopAll();
        this.Delay(FADE_DURATION).FadeOut();

        globalClock.Start();
        globalClock.VolumeIn(FADE_DURATION);
        backgrounds.AddBackgroundFromMap(mapStore.CurrentMapSet?.Maps[0]);
        return false;
    }

    public override void OnResuming(ScreenTransitionEvent e)
    {
        var screen = screenStack.CurrentScreen as MultiSubScreen;
        screen?.OnResuming(null);
    }
}
