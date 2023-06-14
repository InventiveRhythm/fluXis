using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using fluXis.Game.Audio;
using fluXis.Game.Configuration;
using fluXis.Game.Database;
using fluXis.Game.Database.Maps;
using fluXis.Game.Graphics;
using fluXis.Game.Graphics.Background;
using fluXis.Game.Import;
using fluXis.Game.Input;
using fluXis.Game.Integration;
using fluXis.Game.Map;
using fluXis.Game.Online.Fluxel;
using fluXis.Game.Overlay.Chat;
using fluXis.Game.Overlay.Toolbar;
using fluXis.Game.Overlay.Login;
using fluXis.Game.Overlay.Mouse;
using fluXis.Game.Overlay.Notification;
using fluXis.Game.Overlay.Profile;
using fluXis.Game.Overlay.Register;
using fluXis.Game.Overlay.Settings;
using fluXis.Game.Screens;
using fluXis.Game.Screens.Menu;
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

    public AudioClock AudioClock;
    public FluXisRealm Realm;
    public MapStore MapStore;
    public FluXisConfig Config;
    public BackgroundStack BackgroundStack;
    public GlobalCursorOverlay CursorOverlay;
    public SettingsMenu Settings;
    public NotificationOverlay Notifications;
    public LoginOverlay LoginOverlay;
    public ChatOverlay ChatOverlay;
    public RegisterOverlay RegisterOverlay;
    public Toolbar Toolbar;
    public FluXisScreenStack ScreenStack;
    public ProfileOverlay ProfileOverlay;
    public LightController LightController;
    public SkinManager SkinManager;
    public ImportManager ImportManager;

    public Action OnSongChanged;

    public static string VersionString => version != null ? isDebug ? "local development build" : $"v{version.Major}.{version.Minor}.{version.Build}" : "unknown version";
    private static Version version => Assembly.GetEntryAssembly()?.GetName().Version;
    private static bool isDebug => Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyConfigurationAttribute>()?.Configuration == "Debug";

    public virtual LightController CreateLightController() => new();

    protected FluXisGameBase()
    {
    }

    [BackgroundDependencyLoader]
    private void load(Storage storage)
    {
        Resources.AddStore(new DllResourceStore(FluXisResources.ResourceAssembly));
        InitFonts();

        dependencies.CacheAs(this);
        dependencies.CacheAs(Config = new FluXisConfig(storage));
        dependencies.Cache(Realm = new FluXisRealm(storage));
        dependencies.Cache(Notifications = new NotificationOverlay());
        dependencies.Cache(MapStore = new MapStore(storage, Realm));

        LoadComponent(ImportManager = new ImportManager());
        dependencies.Cache(ImportManager);

        dependencies.Cache(AudioClock = new AudioClock());
        dependencies.Cache(new BackgroundTextureStore(Host, storage));
        dependencies.Cache(BackgroundStack = new BackgroundStack());
        dependencies.Cache(CursorOverlay = new GlobalCursorOverlay());
        dependencies.Cache(Settings = new SettingsMenu());
        dependencies.Cache(LoginOverlay = new LoginOverlay());
        dependencies.Cache(ChatOverlay = new ChatOverlay());
        dependencies.CacheAs(RegisterOverlay = new RegisterOverlay());
        dependencies.Cache(Toolbar = new Toolbar());
        dependencies.Cache(ScreenStack = new FluXisScreenStack());
        dependencies.Cache(ProfileOverlay = new ProfileOverlay());
        dependencies.CacheAs(LightController = CreateLightController());
        dependencies.Cache(SkinManager = new SkinManager());

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
                    keybinds = new FluXisKeybindContainer(this, Realm),
                    LightController,
                    SkinManager,
                    content = new Container
                    {
                        RelativeSizeAxes = Axes.Both
                    }
                }
            }
        });

        dependencies.Cache(keybinds);
        MenuSplashes.Load(storage);
    }

    protected void InitFonts()
    {
        AddFont(Resources, @"Fonts/Quicksand/Quicksand");
        AddFont(Resources, @"Fonts/Quicksand/Quicksand-SemiBold");
        AddFont(Resources, @"Fonts/Quicksand/Quicksand-Bold");
        AddFont(Resources, @"Fonts/Renogare/Renogare");
        AddFont(Resources, @"Fonts/YoureGone/YoureGone");
    }

    public new virtual void Exit()
    {
        Fluxel.Close();
        base.Exit();
    }

    public override void SetHost(GameHost host)
    {
        base.SetHost(host);

        host.ExceptionThrown += e =>
        {
            exceptionCount++;
            Task.Delay(1000).ContinueWith(_ => exceptionCount--);

            Notifications.PostError($"An unhandled error occurred!\n{e.GetType().Name}:\n{e.Message}", 10000);
            return exceptionCount <= 1;
        };
    }

    protected override bool OnExiting()
    {
        Fluxel.Close();
        return base.OnExiting();
    }

    public void NextSong() => changeSong(1);
    public void PreviousSong() => changeSong(-1);

    private void changeSong(int change)
    {
        int index = MapStore.MapSets.IndexOf(MapStore.CurrentMapSet);

        index += change;

        if (index >= MapStore.MapSets.Count)
            index = 0;
        else if (index < 0)
            index = MapStore.MapSets.Count - 1;

        RealmMapSet mapSet = MapStore.MapSets[index];
        RealmMap map = mapSet.Maps.First();

        MapStore.CurrentMapSet = mapSet;
        BackgroundStack.AddBackgroundFromMap(map);
        AudioClock.LoadMap(map, true);
        OnSongChanged?.Invoke();
    }

    protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent) => dependencies = new DependencyContainer(base.CreateChildDependencies(parent));
}
