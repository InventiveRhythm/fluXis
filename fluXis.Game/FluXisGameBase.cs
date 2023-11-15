using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
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
using fluXis.Game.Online.Activity;
using fluXis.Game.Online.Fluxel;
using fluXis.Game.Overlay.Chat;
using fluXis.Game.Overlay.Toolbar;
using fluXis.Game.Overlay.Login;
using fluXis.Game.Overlay.Mouse;
using fluXis.Game.Overlay.Music;
using fluXis.Game.Overlay.Network;
using fluXis.Game.Overlay.Notifications;
using fluXis.Game.Overlay.Profile;
using fluXis.Game.Overlay.Register;
using fluXis.Game.Overlay.Settings;
using fluXis.Game.Screens;
using fluXis.Game.Screens.Gameplay.HUD;
using fluXis.Game.Screens.Menu;
using fluXis.Game.Screens.Skin;
using fluXis.Game.Skinning;
using fluXis.Game.Updater;
using fluXis.Resources;
using Newtonsoft.Json;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
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

    protected AudioClock AudioClock { get; private set; }
    protected GlobalCursorOverlay CursorOverlay { get; private set; }
    protected LoginOverlay LoginOverlay { get; private set; }
    protected ChatOverlay ChatOverlay { get; private set; }
    protected ProfileOverlay ProfileOverlay { get; private set; }
    protected RegisterOverlay RegisterOverlay { get; private set; }
    protected NotificationManager NotificationManager { get; private set; }
    protected MusicPlayer MusicPlayer { get; private set; }
    protected Dashboard Dashboard { get; private set; }
    protected BackgroundStack BackgroundStack { get; private set; }
    protected UISamples Samples { get; private set; }
    protected Fluxel Fluxel { get; private set; }

    public SettingsMenu Settings { get; private set; }
    public Toolbar Toolbar { get; private set; }
    public FluXisScreenStack ScreenStack { get; private set; }
    public MenuScreen MenuScreen { get; protected set; }

    private FluXisRealm realm;
    private MapStore mapStore;
    private KeybindStore keybindStore;
    private FluXisConfig config;
    private LightController lightController;
    private SkinManager skinManager;
    private ImportManager importManager;

    protected Bindable<UserActivity> Activity { get; } = new();

    public Action OnSongChanged { get; set; }
    public virtual Drawable Overlay { get; set; }

    public static string VersionString => Version != null ? IsDebug ? "local development build" : $"v{Version.Major}.{Version.Minor}.{Version.Build}" : "unknown version";
    public static Version Version => Assembly.GetEntryAssembly()?.GetName().Version;
    public static bool IsDebug => Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyConfigurationAttribute>()?.Configuration == "Debug";

    public virtual LightController CreateLightController() => new();
    public virtual IUpdateManager CreateUpdateManager() => null;

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
        dependencies.Cache(NotificationManager = new NotificationManager());
        dependencies.Cache(Fluxel = new Fluxel(config, getApiEndpoint()));
        UserCache.Init(Fluxel);

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
        dependencies.Cache(ScreenStack = new FluXisScreenStack { Activity = Activity });
        dependencies.Cache(ProfileOverlay = new ProfileOverlay());
        dependencies.CacheAs(lightController = CreateLightController());
        dependencies.Cache(skinManager = new SkinManager());
        dependencies.Cache(MusicPlayer = new MusicPlayer { ScreenStack = ScreenStack });
        dependencies.Cache(Dashboard = new Dashboard());

        var layoutManager = new LayoutManager();
        LoadComponent(layoutManager);
        dependencies.Cache(layoutManager);

        Textures.AddTextureSource(Host.CreateTextureLoaderStore(new HttpOnlineStore()));

        GlobalKeybindContainer keybinds;

        base.Content.Add(new SafeAreaContainer
        {
            RelativeSizeAxes = Axes.Both,
            Child = new DrawSizePreservingFillContainer
            {
                TargetDrawSize = new Vector2(1920, 1080),
                RelativeSizeAxes = Axes.Both,
                Children = new Drawable[]
                {
                    keybinds = new GlobalKeybindContainer(this, realm)
                    {
                        Children = new Drawable[]
                        {
                            lightController,
                            skinManager,
                            content = new Container
                            {
                                RelativeSizeAxes = Axes.Both
                            }
                        }
                    },
                    new GamepadHandler()
                }
            }
        });

        keybindStore = new KeybindStore(realm);
        keybindStore.AssignDefaults(keybinds);
        keybindStore.AssignDefaults(new GameplayKeybindContainer(this, realm));

        dependencies.Cache(keybinds);
        MenuSplashes.Load(storage);
    }

    public void PerformUpdateCheck(bool silent, bool forceUpdate = false) => Task.Run(() => CreateUpdateManager()?.Perform(silent, forceUpdate));

    private APIEndpointConfig getApiEndpoint()
    {
        var path = Host.Storage.GetFullPath("server.json");

        if (File.Exists(path))
        {
            var json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<APIEndpointConfig>(json);
        }

        var endpoint = new APIEndpointConfig
        {
            APIUrl = "https://api.fluxis.flux.moe",
            WebsocketUrl = "wss://fluxel.flux.moe",
            WebsiteRootUrl = "https://fluxis.flux.moe"
        };

        var json2 = JsonConvert.SerializeObject(endpoint, Formatting.Indented);
        File.WriteAllText(path, json2);

        return getApiEndpoint();
    }

    private void initFonts()
    {
        AddFont(Resources, "Fonts/Renogare/Renogare");
        AddFont(Resources, "Fonts/RenogareSoft/RenogareSoft");
        AddFont(Resources, "Fonts/YoureGone/YoureGone");
    }

    public new virtual void Exit()
    {
        Fluxel.Close();
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

            NotificationManager.SendError("An unhandled error occurred!", e.Message, FontAwesome.Solid.Bomb);
            return exceptionCount <= MaxExceptions;
        };
    }

    protected override bool OnExiting()
    {
        Fluxel.Close();
        return base.OnExiting();
    }

    public void OpenSkinEditor()
    {
        if (ScreenStack.CurrentScreen is SkinEditor)
        {
            NotificationManager.SendText("You are already in the Skin editor.", "", FontAwesome.Solid.Bomb);
            return;
        }

        if (skinManager.SkinFolder == "Default")
        {
            NotificationManager.SendError("You can't edit the Default skin.");
            return;
        }

        Settings.Hide();
        ScreenStack.Push(new SkinEditor());
    }

    public void NextSong() => changeSong(1);
    public void PreviousSong() => changeSong(-1);

    private void changeSong(int change)
    {
        if (mapStore.MapSets.Count <= 0)
            return;

        int index = mapStore.MapSets.IndexOf(mapStore.CurrentMapSet);

        index += change;

        if (index >= mapStore.MapSets.Count)
            index = 0;
        else if (index < 0)
            index = mapStore.MapSets.Count - 1;

        RealmMapSet set = mapStore.MapSets[index];
        SelectMapSet(set);
    }

    public void SelectMapSet(RealmMapSet set)
    {
        mapStore.CurrentMapSet = set;
        var map = set.Maps.First();
        BackgroundStack.AddBackgroundFromMap(map);
        AudioClock.LoadMap(map, true);
    }

    protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent) => dependencies = new DependencyContainer(base.CreateChildDependencies(parent));
}
