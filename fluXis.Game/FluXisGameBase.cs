using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using fluXis.Game.Activity;
using fluXis.Game.Audio;
using fluXis.Game.Configuration;
using fluXis.Game.Database;
using fluXis.Game.Database.Maps;
using fluXis.Game.Graphics;
using fluXis.Game.Graphics.Background;
using fluXis.Game.Graphics.Background.Cropped;
using fluXis.Game.Import;
using fluXis.Game.Input;
using fluXis.Game.Integration;
using fluXis.Game.Map;
using fluXis.Game.Online;
using fluXis.Game.Online.Fluxel;
using fluXis.Game.Overlay.Chat;
using fluXis.Game.Overlay.Toolbar;
using fluXis.Game.Overlay.Login;
using fluXis.Game.Overlay.Mouse;
using fluXis.Game.Overlay.Music;
using fluXis.Game.Overlay.Network;
using fluXis.Game.Overlay.Notification;
using fluXis.Game.Overlay.Profile;
using fluXis.Game.Overlay.Register;
using fluXis.Game.Overlay.Settings;
using fluXis.Game.Screens;
using fluXis.Game.Screens.Menu;
using fluXis.Game.Screens.Skin;
using fluXis.Game.Skinning;
using fluXis.Resources;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.IO.Stores;
using osu.Framework.Platform;
using osuTK;

namespace fluXis.Game;

public partial class FluXisGameBase : osu.Framework.Game
{
    // Anything in this class is shared between the test browser and the game implementation.
    // It allows for caching global dependencies that should be accessible to tests, or changing
    // the screen scaling for all components including the test browser and framework overlays.

    private DependencyContainer dependencies;

    protected override Container<Drawable> Content => content;
    private Container content;
    private int exceptionCount;
    protected virtual int MaxExceptions => IsDebug ? 5 : 1;

    protected AudioClock AudioClock;
    protected GlobalCursorOverlay CursorOverlay;
    protected LoginOverlay LoginOverlay;
    protected ChatOverlay ChatOverlay;
    protected ProfileOverlay ProfileOverlay;
    protected RegisterOverlay RegisterOverlay;
    protected NotificationOverlay Notifications;
    protected MusicPlayer MusicPlayer;
    protected Dashboard Dashboard;
    protected BackgroundStack BackgroundStack;
    protected ActivityManager ActivityManager;
    protected UISamples Samples;

    public SettingsMenu Settings;
    public Toolbar Toolbar;
    public FluXisScreenStack ScreenStack;

    private FluXisRealm realm;
    private MapStore mapStore;
    private FluXisConfig config;
    private LightController lightController;
    private SkinManager skinManager;
    private ImportManager importManager;
    private Fluxel fluxel;

    public Action OnSongChanged { get; set; }
    public virtual Drawable Overlay { get; set; }

    public static string VersionString => Version != null ? IsDebug ? "local development build" : $"v{Version.Major}.{Version.Minor}.{Version.Build}" : "unknown version";
    public static Version Version => Assembly.GetEntryAssembly()?.GetName().Version;
    public static bool IsDebug => Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyConfigurationAttribute>()?.Configuration == "Debug";

    public virtual LightController CreateLightController() => new();

    protected FluXisGameBase()
    {
    }

    [BackgroundDependencyLoader]
    private void load(Storage storage)
    {
        Resources.AddStore(new DllResourceStore(FluXisResources.ResourceAssembly));
        initFonts();

        MapFiles.Initialize(storage.GetStorageForDirectory("maps"));

        dependencies.CacheAs(this);
        dependencies.CacheAs(config = new FluXisConfig(storage));
        dependencies.Cache(realm = new FluXisRealm(storage));
        dependencies.Cache(Notifications = new NotificationOverlay());
        dependencies.Cache(fluxel = new Fluxel(config, getApiEndpoint()));
        UserCache.Init(fluxel);

        dependencies.Cache(new BackgroundTextureStore(Host, storage.GetStorageForDirectory("maps")));
        dependencies.Cache(new CroppedBackgroundStore(Host, storage.GetStorageForDirectory("maps")));

        LoadComponent(mapStore = new MapStore());
        dependencies.Cache(mapStore);

        LoadComponent(importManager = new ImportManager());
        dependencies.Cache(importManager);

        LoadComponent(Samples = new UISamples());
        dependencies.Cache(Samples);

        dependencies.Cache(AudioClock = new AudioClock());
        dependencies.Cache(BackgroundStack = new BackgroundStack());
        dependencies.Cache(CursorOverlay = new GlobalCursorOverlay());
        dependencies.Cache(Settings = new SettingsMenu());
        dependencies.Cache(LoginOverlay = new LoginOverlay());
        dependencies.Cache(ChatOverlay = new ChatOverlay());
        dependencies.CacheAs(RegisterOverlay = new RegisterOverlay());
        dependencies.Cache(Toolbar = new Toolbar());
        dependencies.Cache(ScreenStack = new FluXisScreenStack());
        dependencies.Cache(ProfileOverlay = new ProfileOverlay());
        dependencies.CacheAs(lightController = CreateLightController());
        dependencies.Cache(skinManager = new SkinManager());
        dependencies.Cache(ActivityManager = new ActivityManager(fluxel));
        dependencies.Cache(MusicPlayer = new MusicPlayer { ScreenStack = ScreenStack });
        dependencies.Cache(Dashboard = new Dashboard());

        Textures.AddTextureSource(Host.CreateTextureLoaderStore(new HttpOnlineStore()));

        FluXisKeybindContainer keybinds;

        base.Content.Add(new SafeAreaContainer
        {
            RelativeSizeAxes = Axes.Both,
            Child = new DrawSizePreservingFillContainer
            {
                TargetDrawSize = new Vector2(1920, 1080),
                RelativeSizeAxes = Axes.Both,
                Children = new Drawable[]
                {
                    lightController,
                    skinManager,
                    content = new Container
                    {
                        RelativeSizeAxes = Axes.Both
                    },
                    keybinds = new FluXisKeybindContainer(this, realm),
                    new GamepadHandler()
                }
            }
        });

        dependencies.Cache(keybinds);
        MenuSplashes.Load(storage);
    }

    private APIEndpointConfig getApiEndpoint()
    {
        return config.Get<bool>(FluXisSetting.UseDebugServer)
            ? new APIEndpointConfig
            {
                APIUrl = "http://localhost:2434",
                WebsocketUrl = "ws://localhost:2435",
                WebsiteRootUrl = "https://fluxis.foxes4life.net"
            }
            : new APIEndpointConfig
            {
                APIUrl = "https://api.fluxis.foxes4life.net",
                WebsocketUrl = "wss://fluxel.foxes4life.net",
                WebsiteRootUrl = "https://fluxis.foxes4life.net"
            };
    }

    private void initFonts()
    {
        AddFont(Resources, "Fonts/Renogare/Renogare");
        AddFont(Resources, "Fonts/RenogareSoft/RenogareSoft");
        AddFont(Resources, "Fonts/YoureGone/YoureGone");
    }

    public new virtual void Exit()
    {
        fluxel.Close();
        base.Exit();
    }

    public void HandleDragDrop(string[] files) => importManager.ImportMultiple(files);
    public void HandleDragDrop(string file) => importManager.Import(file);

    public override void SetHost(GameHost host)
    {
        base.SetHost(host);

        host.ExceptionThrown += e =>
        {
            exceptionCount++;
            Task.Delay(1000).ContinueWith(_ => exceptionCount--);

            Notifications.PostError($"An unhandled error occurred!\n{e.GetType().Name}:\n{e.Message}", 10000);
            return exceptionCount <= MaxExceptions;
        };
    }

    protected override bool OnExiting()
    {
        fluxel.Close();
        return base.OnExiting();
    }

    public void OpenSkinEditor()
    {
        if (ScreenStack.CurrentScreen is not MenuScreen)
        {
            Notifications.Post(ScreenStack.CurrentScreen is SkinEditor
                ? "You are already in the Skin editor."
                : "You can only open the Skin editor from the main menu.");
            return;
        }

        if (skinManager.SkinFolder == "Default")
        {
            Notifications.Post("You can't edit the Default skin.");
            return;
        }

        Settings.Hide();
        ScreenStack.Push(new SkinEditor());
    }

    public void NextSong() => changeSong(1);
    public void PreviousSong() => changeSong(-1);

    private void changeSong(int change)
    {
        int index = mapStore.MapSets.IndexOf(mapStore.CurrentMapSet);

        index += change;

        if (index >= mapStore.MapSets.Count)
            index = 0;
        else if (index < 0)
            index = mapStore.MapSets.Count - 1;

        RealmMapSet mapSet = mapStore.MapSets[index];
        RealmMap map = mapSet.Maps.First();

        mapStore.CurrentMapSet = mapSet;
        BackgroundStack.AddBackgroundFromMap(map);
        AudioClock.LoadMap(map, true);
    }

    protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent) => dependencies = new DependencyContainer(base.CreateChildDependencies(parent));
}
