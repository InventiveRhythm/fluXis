using System.Linq;
using fluXis.Audio;
using fluXis.Configuration;
using fluXis.Graphics;
using fluXis.Graphics.Sprites;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Panel;
using fluXis.Graphics.UserInterface.Panel.Types;
using fluXis.Map;
using fluXis.Online.Multiplayer;
using fluXis.Screens.Multiplayer.SubScreens;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
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
    protected override bool BackgroundAllowStoryboard => showStoryboard.Value && client.Connected && client.Player.IsSupporter;
    public override bool AllowMusicControl => false;
    public override bool AllowMusicPausing => screenStack?.CurrentScreen is MultiSubScreen { AllowMusicPausing: true };
    public override bool AllowExit => canExit();

    [Resolved]
    private GlobalClock globalClock { get; set; }

    [Resolved]
    private MapStore mapStore { get; set; }

    [Resolved]
    private PanelContainer panels { get; set; }

    [Cached]
    private MultiplayerMenuMusic menuMusic = new();

    public long TargetLobby { get; init; }
    public string LobbyPassword { get; init; }

    private Bindable<bool> showStoryboard;

    private ScreenStack screenStack;
    private DependencyContainer dependencies;
    private MultiplayerClient client;

    private FillFlowContainer connectingContainer;

    [BackgroundDependencyLoader]
    private void load(FluXisConfig config)
    {
        showStoryboard = config.GetBindable<bool>(FluXisSetting.ShowStoryboardVideo);

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
            connectingContainer.FadeOut(Styling.TRANSITION_FADE);
        });

        LoadComponentAsync(client, _ =>
        {
            if (!IsPresent || !client.Connected)
            {
                client.Dispose();
                return;
            }

            AddInternal(client);
            connectingContainer.FadeOut(Styling.TRANSITION_FADE);

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
        while (true)
        {
            var screen = screenStack.CurrentScreen as Screen;

            if (screen is null or MultiModeSelect)
                return true;

            if (screen.IsLoaded && screen.OnExiting(null))
                return false;

            screen.Exit();
        }
    }

    public override void OnEntering(ScreenTransitionEvent e)
    {
        using (BeginDelayedSequence(Styling.TRANSITION_ENTER_DELAY))
        {
            screenStack.FadeIn(Styling.TRANSITION_FADE);
            connectingContainer.FadeIn(Styling.TRANSITION_FADE);
        }

        globalClock.VolumeOut(400).OnComplete(c => c.Stop());
        ApplyMapBackground(null);
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
        this.Delay(Styling.TRANSITION_FADE).FadeOut();

        globalClock.Start();
        globalClock.VolumeIn(Styling.TRANSITION_FADE);
        ApplyMapBackground(mapStore.CurrentMapSet?.Maps.First());
        return false;
    }
}
