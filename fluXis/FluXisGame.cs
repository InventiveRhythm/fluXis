using System;
using System.Linq;
using fluXis.Audio;
using fluXis.Audio.Transforms;
using fluXis.Configuration;
using fluXis.Database.Maps;
using fluXis.Graphics.Background;
using fluXis.Graphics.Sprites;
using fluXis.Graphics.UserInterface.Panel;
using fluXis.Graphics.UserInterface.Panel.Presets;
using fluXis.Input;
using fluXis.Localization;
using fluXis.Localization.Stores;
using fluXis.Online.API.Models.Users;
using fluXis.Overlay.Achievements;
using fluXis.Overlay.Auth;
using fluXis.Overlay.Browse;
using fluXis.Overlay.Chat;
using fluXis.Overlay.Club;
using fluXis.Overlay.Exit;
using fluXis.Overlay.FPS;
using fluXis.Overlay.MapSet;
using fluXis.Overlay.Music;
using fluXis.Overlay.Network;
using fluXis.Overlay.Notifications;
using fluXis.Overlay.Notifications.Tasks;
using fluXis.Overlay.Notifications.Types.Image;
using fluXis.Overlay.Settings;
using fluXis.Overlay.Toolbar;
using fluXis.Overlay.User;
using fluXis.Overlay.Volume;
using fluXis.Overlay.Wiki;
using fluXis.Scoring;
using fluXis.Screens;
using fluXis.Screens.Intro;
using fluXis.Screens.Layout;
using fluXis.Screens.Loading;
using fluXis.Screens.Menu;
using fluXis.Screens.Multiplayer;
using fluXis.Screens.Result;
using fluXis.Screens.Select;
using fluXis.Screens.Skinning;
using fluXis.Utils;
using fluXis.Utils.Extensions;
using fluXis.Utils.Sentry;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Bindables;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.IO.Stores;
using osu.Framework.Localisation;
using osu.Framework.Logging;
using osu.Framework.Screens;

namespace fluXis;

public partial class FluXisGame : FluXisGameBase, IKeyBindingHandler<FluXisGlobalKeybind>
{
    public static readonly string[] AUDIO_EXTENSIONS = { ".mp3", ".wav", ".ogg" };
    public static readonly string[] IMAGE_EXTENSIONS = { ".jpg", ".jpeg", ".png" };
    public static readonly string[] VIDEO_EXTENSIONS = { ".mp4", ".mov", ".avi", ".flv", ".mpg", ".wmv", ".m4v" };

    protected override bool LoadComponentsLazy => true;

    private BufferedContainer buffer;
    private GlobalClock globalClock;
    private GlobalBackground globalBackground;
    private Container screenContainer;
    private FluXisScreenStack screenStack;
    private Container<VisibilityContainer> overlayContainer;
    private Dashboard dashboard;
    private UserProfileOverlay userProfileOverlay;
    private MapSetOverlay mapSetOverlay;
    private Toolbar toolbar;
    private PanelContainer panelContainer;
    private FloatingNotificationContainer notificationContainer;
    private ExitAnimation exitAnimation;

    private SentryClient sentry { get; }

    private bool isExiting;

    private readonly BindableDouble inactiveVolume = new(1f);

    public bool AnyOverlayOpen => overlayContainer.Any(x => x.State.Value == Visibility.Visible);
    private Bindable<bool> allowOverlays { get; } = new(true);

    [UsedImplicitly]
    public bool Sex { get; private set; }

    public FluXisGame()
    {
        // created here so that we can catch events before the game even starts
        sentry = new SentryClient(this);
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        if (LoadFailed)
            return;

        GameDependencies.CacheAs(this);

        Children = new Drawable[]
        {
            buffer = new BufferedContainer
            {
                RelativeSizeAxes = Axes.Both,
                RedrawOnScale = false
            }
        };

        loadComponent(sentry, _ => { }, true);
        loadComponent(globalClock = new GlobalClock(), Add, true);
        GameDependencies.CacheAs<IBeatSyncProvider>(globalClock);
        GameDependencies.CacheAs<IAmplitudeProvider>(globalClock);

        loadComponent(NotificationManager, Add);

        loadComponent(globalBackground = new GlobalBackground { InitialDim = 1 }, buffer.Add, true);
        loadComponent(screenContainer = new Container { RelativeSizeAxes = Axes.Both }, buffer.Add);
        loadComponent(screenStack = new FluXisScreenStack(APIClient.Activity, allowOverlays), screenContainer.Add, true);

        loadComponent(overlayContainer = new Container<VisibilityContainer> { RelativeSizeAxes = Axes.Both }, buffer.Add);
        loadComponent(dashboard = new Dashboard(), overlayContainer.Add, true);
        loadComponent(new ChatOverlay(), overlayContainer.Add, true);
        loadComponent(new BrowseOverlay(), overlayContainer.Add, true);
        loadComponent(mapSetOverlay = new MapSetOverlay(), overlayContainer.Add, true);
        loadComponent(userProfileOverlay = new UserProfileOverlay(), overlayContainer.Add, true);
        loadComponent(new WikiOverlay(), overlayContainer.Add, true);
        loadComponent(new ClubOverlay(), overlayContainer.Add, true);
        loadComponent(new MusicPlayer(), overlayContainer.Add, true);
        loadComponent(new SettingsMenu(), overlayContainer.Add, true);

        loadComponent(new LoginOverlay(), buffer.Add, true);
        loadComponent(new RegisterOverlay(), buffer.Add, true);
        loadComponent(new MultifactorOverlay(), buffer.Add, true);
        loadComponent(toolbar = new Toolbar(), buffer.Add, true);

        loadComponent(panelContainer = new PanelContainer { BlurContainer = buffer }, Add, true);
        loadComponent(new VolumeOverlay(), Add);

        NotificationManager.Floating = loadComponent(notificationContainer = new FloatingNotificationContainer(), Add);
        NotificationManager.Tasks = loadComponent(new TaskNotificationContainer(), Add);

        loadComponent(new FpsOverlay(), Add);
        loadComponent(exitAnimation = new ExitAnimation(), Add);

        loadComponent(MenuScreen = new MenuScreen());

#if VELOPACK_BUILD
        if (CanUpdate)
            LoadQueue.Push(new LoadTask("Checking for updates...", complete => PerformUpdateCheck(true, () => Schedule(complete))));
#endif

        LoadQueue.Push(new LoadTask("Checking for bundled maps...", c =>
        {
            if (MapStore.MapSets.Count > 0)
            {
                c?.Invoke();
                return;
            }

            MapStore.DownloadBundledMaps(c);
        }));

        Audio.AddAdjustment(AdjustableProperty.Volume, inactiveVolume);

        IsActive.BindValueChanged(active =>
        {
            var volume = Config.Get<double>(FluXisSetting.InactiveVolume);
            this.TransformBindableTo(inactiveVolume, active.NewValue ? 1 : volume, active.NewValue ? 500 : 4000, Easing.OutQuint);
        }, true);
    }

    private T loadComponent<T>(T component, Action<T> action = null, bool cache = false, bool preload = false)
        where T : Drawable
    {
        if (cache)
            GameDependencies.CacheAs(component);

        if (preload)
        {
            action?.Invoke(component);
            return component;
        }

        CreateComponentLoadTask(component, action);
        Scheduler.AddOnce(loadNext);
        return component;
    }

    private void loadNext()
    {
        if (screenStack.CurrentScreen is not LoadingScreen)
        {
            Schedule(loadNext);
            return;
        }

        LoadQueue.PerformNext(loadNext);
    }

    public void WaitForReady(Action action)
    {
        if (screenStack?.CurrentScreen is null or LoadingScreen or IntroAnimation)
            Schedule(() => WaitForReady(action));
        else
            action();
    }

    protected override void LoadComplete()
    {
        if (LoadFailed)
            return;

        base.LoadComplete();

        sentry.BindUser(APIClient.User);

        loadLocales();

        toolbar.AllowOverlays.BindTo(allowOverlays);

        allowOverlays.ValueChanged += e =>
        {
            if (!e.NewValue)
                CloseOverlays();
        };

        ScheduleAfterChildren(() => screenStack.Push(new LoadingScreen(LoadQueue)));

        APIClient.FriendOnline += u => Schedule(() => NotificationManager.SendSmallText($"{u.PreferredName} is now online!", FontAwesome6.Solid.UserPlus));
        APIClient.FriendOffline += u => Schedule(() => NotificationManager.SendSmallText($"{u.PreferredName} is now offline!", FontAwesome6.Solid.UserMinus));
        APIClient.AchievementEarned += a => Schedule(() => LoadComponentAsync(new AchievementOverlay(a), ov => Schedule(() => panelContainer.Content = ov)));
        APIClient.NameChangeRequested += () => WaitForReady(() => Schedule(() => panelContainer.Content = new UsernameChangePanel()));

        APIClient.MessageReceived += message =>
        {
            switch (message.Type)
            {
                case "normal":
                    NotificationManager.SendText(message.Text, message.SubText);
                    break;

                case "small":
                    NotificationManager.SendSmallText(message.Text);
                    break;

                case "image":
                    NotificationManager.Add(new ImageNotificationData
                    {
                        Text = message.Text,
                        Path = message.Path,
                        Location = ImageNotificationData.ImageLocation.Online
                    });
                    break;
            }
        };
    }

    public override void SelectMapSet(RealmMapSet set)
    {
        base.SelectMapSet(set);
        globalBackground.AddBackgroundFromMap(set.Maps.First());
    }

    public void OpenLink(string link, bool skipWarning = false)
    {
        if (skipWarning)
        {
            if (Steam?.Initialized ?? false)
                Steam.OpenLink(link);
            else
                Host.OpenUrlExternally(link);

            return;
        }

        if (panelContainer.Content != null)
        {
            var panel = panelContainer.Content as Panel;
            panel?.Flash();
            return;
        }

        panelContainer.Content = new ExternalLinkPanel(link);
    }

    public void OpenDashboard(DashboardTabType type)
    {
        CloseOverlays();
        dashboard.Show(type);
    }

    public void OpenSkinEditor()
    {
        if (screenStack.CurrentScreen is SkinEditor)
            return;

        if (SkinManager.IsDefault)
            return;

        CloseOverlays();
        screenStack.Push(new SkinEditor());
    }

    public void OpenLayoutEditor()
    {
        if (screenStack.CurrentScreen is LayoutEditor)
            return;

        if (LayoutManager.IsDefault)
            return;

        CloseOverlays();
        screenStack.Push(new LayoutEditor(LayoutManager.Layout.Value));
    }

    public override void CloseOverlays() => overlayContainer.Children.ForEach(c => c.Hide());

    public override void PresentScore(RealmMap map, ScoreInfo score, APIUser player, Action replayAction = null)
    {
        if (map == null || score == null)
            throw new ArgumentNullException();

        screenStack.Push(new Results(map, score, player) { ViewReplay = replayAction });
    }

    public void PresentUser(long id)
        => userProfileOverlay.ShowUser(id);

    public void PresentMapSet(long id)
    {
        mapSetOverlay.ShowSet(id);
        userProfileOverlay.Hide();
    }

    public override void ShowMap(RealmMapSet set)
    {
        CloseOverlays();
        SelectMapSet(set);

        if (screenStack.CurrentScreen is not SelectScreen)
        {
            MenuScreen.MakeCurrent();

            if (MenuScreen.IsCurrentScreen())
                MenuScreen.Push(new SoloSelectScreen());
        }
    }

    public void JoinMultiplayerRoom(long id, string password) => Scheduler.ScheduleIfNeeded(() => WaitForReady(() =>
    {
        MenuScreen.MakeCurrent();

        if (!MenuScreen.IsCurrentScreen())
            return;

        MenuScreen.CanPlayAnimation();

        if (MenuScreen.IsCurrentScreen())
        {
            MenuScreen.Push(new MultiplayerScreen
            {
                TargetLobby = id,
                LobbyPassword = password
            });
        }
    }));

    public bool OnPressed(KeyBindingPressEvent<FluXisGlobalKeybind> e)
    {
        if (e.Repeat) return false;

        switch (e.Action)
        {
            case FluXisGlobalKeybind.OpenSkinEditor:
                OpenSkinEditor();
                return true;

            case FluXisGlobalKeybind.Funny when !Sex:
                NotificationManager.SendSmallText("Sex mode activated!");
                Sex = true;
                return true;
        }

        switch (e.Action)
        {
            case FluXisGlobalKeybind.MusicPause when screenStack.AllowMusicPausing:
                if (globalClock.IsRunning) globalClock.Stop();
                else globalClock.Start();
                return true;

            case FluXisGlobalKeybind.MusicPrevious when screenStack.AllowMusicControl:
                PreviousSong();
                return true;

            case FluXisGlobalKeybind.MusicNext when screenStack.AllowMusicControl:
                NextSong();
                return true;
        }

        return false;
    }

    public void OnReleased(KeyBindingReleaseEvent<FluXisGlobalKeybind> e) { }

    protected override void Update()
    {
        if (LoadFailed)
            return;

        screenContainer.Padding = new MarginPadding { Top = toolbar.Height + toolbar.Y };
        notificationContainer.Y = toolbar.Height + toolbar.Y;

        CursorOverlay.ShowCursor = screenStack.CurrentScreen is not FluXisScreen screen || screen.ShowCursor;

        if (globalClock.Finished && screenStack.CurrentScreen is FluXisScreen { AutoPlayNext: true })
            NextSong();
    }

    protected override bool OnExiting()
    {
        if (LoadFailed)
            return base.OnExiting();

        if (panelContainer.Content != null && panelContainer.Content is not ConfirmExitPanel)
        {
            Logger.Log("Blocking exit due to panel being open.", LoggingTarget.Runtime, LogLevel.Debug);
            var panel = panelContainer.Content as Panel;
            panel?.Flash();
            return true;
        }

        if (screenStack.CurrentScreen is not Screens.Menu.MenuScreen)
        {
            Logger.Log("Blocking exit due to non-mainmenu screen being open.", LoggingTarget.Runtime, LogLevel.Debug);
            MenuScreen.MakeCurrent();
            panelContainer.Content = new ConfirmExitPanel();
            return true;
        }

        Logger.Log("Exiting...", LoggingTarget.Runtime, LogLevel.Debug);

        panelContainer.Content?.Hide();
        Schedule(Exit);
        return !isExiting;
    }

    public override void Exit(bool restart)
    {
#if VELOPACK_BUILD
        if (restart && !RestartOnClose())
            return;
#endif

        toolbar.Hide();
        globalClock.RateTo(0, 1500, Easing.Out);
        globalClock.VolumeOut(1300);
        exitAnimation.Show(buffer.Hide, () => base.Exit(false));
        isExiting = true;
    }

    private void loadLocales()
    {
        var localeStore = new NamespacedResourceStore<byte[]>(Resources, "Localization");
        localeStore.AddExtension("json");

        var languages = Enum.GetValues<Language>();

        var missingBindable = Config.GetBindable<bool>(FluXisSetting.ShowMissingLocalizations);

        var mappings = languages.Select(l =>
        {
            if (l == Language.debug) return new LocaleMapping("debug", new DebugLocaleStore());

            var code = l.ToCultureCode();

            try
            {
                return new LocaleMapping(code, new ResourceLocaleStore(code, localeStore, missingBindable));
            }
            catch
            {
                return null;
            }
        }).Where(m => m != null);

        Localisation.AddLocaleMappings(mappings);
    }
}
