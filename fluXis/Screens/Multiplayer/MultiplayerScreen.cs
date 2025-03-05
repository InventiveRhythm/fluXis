using fluXis.Audio;
using fluXis.Graphics.Background;
using fluXis.Graphics.Sprites;
using fluXis.Graphics.UserInterface;
using fluXis.Graphics.UserInterface.Panel;
using fluXis.Graphics.UserInterface.Panel.Types;
using fluXis.Map;
using fluXis.Online.Multiplayer;
using fluXis.Screens.Multiplayer.SubScreens;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Screens;
using osuTK;

namespace fluXis.Screens.Multiplayer;

[Cached]
public partial class MultiplayerScreen : FluXisScreen
{
    public override float Zoom => 1.1f;
    public override float ParallaxStrength => 0;
    public override float BackgroundDim => .5f;
    public override float BackgroundBlur => .2f;
    public override bool AllowMusicControl => false;
    public override bool AllowMusicPausing => screenStack?.CurrentScreen is MultiSubScreen { AllowMusicPausing: true };
    public override bool AllowExit => canExit();

    [Resolved]
    private GlobalClock globalClock { get; set; }

    [Resolved]
    private MapStore mapStore { get; set; }

    [Resolved]
    private GlobalBackground backgrounds { get; set; }

    [Resolved]
    private PanelContainer panels { get; set; }

    [Cached]
    private MultiplayerMenuMusic menuMusic = new();

    public long TargetLobby { get; init; }
    public string LobbyPassword { get; init; }

    private ScreenStack screenStack;
    private DependencyContainer dependencies;
    private MultiplayerClient client;

    private FillFlowContainer connectingContainer;

    [BackgroundDependencyLoader]
    private void load()
    {
        client = new OnlineMultiplayerClient();
        dependencies.CacheAs(client);

        InternalChildren = new Drawable[]
        {
            menuMusic,
            screenStack = new ScreenStack
            {
                RelativeSizeAxes = Axes.Both,
                Alpha = 0
            },
            connectingContainer = new FillFlowContainer
            {
                AutoSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Direction = FillDirection.Vertical,
                Spacing = new Vector2(8),
                Alpha = 0,
                Children = new Drawable[]
                {
                    new LoadingIcon
                    {
                        Size = new Vector2(48),
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre
                    },
                    new FluXisSpriteText
                    {
                        Text = "Connecting to Server...",
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        WebFontSize = 16
                    }
                }
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        client.OnConnectionError += ex => Schedule(() =>
        {
            panels.Content = new SingleButtonPanel(FontAwesome6.Solid.TriangleExclamation, "Failed to connect to multiplayer server.", ex.Message, action: this.Exit);
            connectingContainer.FadeOut(FADE_DURATION);
        });

        LoadComponentAsync(client, _ =>
        {
            if (!IsPresent || !client.Connected)
            {
                client.Dispose();
                return;
            }

            AddInternal(client);
            connectingContainer.FadeOut(FADE_DURATION);

            client.OnDisconnect += () => panels.Content = new DisconnectedPanel(() =>
            {
                if (!this.IsCurrentScreen())
                    this.MakeCurrent();

                this.Exit();
            });

            var modes = new MultiModeSelect();
            screenStack.Push(modes);

            if (TargetLobby > 0)
                modes.OpenList(TargetLobby, LobbyPassword);
        });

        screenStack.ScreenExited += (_, _) =>
        {
            if (screenStack.CurrentScreen == null)
                this.Exit();
        };
    }

    protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
        => dependencies = new DependencyContainer(base.CreateChildDependencies(parent));

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
        {
            screenStack.FadeIn(FADE_DURATION);
            connectingContainer.FadeIn(FADE_DURATION);
        }

        globalClock.VolumeOut(400).OnComplete(c => c.Stop());
        backgrounds.AddBackgroundFromMap(null);
    }

    public override void OnResuming(ScreenTransitionEvent e)
    {
        var screen = screenStack.CurrentScreen as MultiSubScreen;
        screen?.OnResuming(null);
        this.FadeIn();
    }

    public override void OnSuspending(ScreenTransitionEvent e)
    {
        var screen = screenStack.CurrentScreen as MultiSubScreen;
        screen?.OnSuspending(null);
        this.Delay(800).FadeOut();
    }

    public override bool OnExiting(ScreenExitEvent e)
    {
        if (!canExit()) return true;

        var screen = screenStack.CurrentScreen as MultiSubScreen;
        screen?.OnSuspending(null);

        menuMusic.StopAll();
        this.Delay(FADE_DURATION).FadeOut();

        globalClock.Start();
        globalClock.VolumeIn(FADE_DURATION);
        backgrounds.AddBackgroundFromMap(mapStore.CurrentMapSet?.Maps[0]);
        return false;
    }
}
