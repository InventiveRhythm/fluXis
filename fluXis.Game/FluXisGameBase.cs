using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using fluXis.Game.Audio;
using fluXis.Game.Audio.Preview;
using fluXis.Game.Configuration;
using fluXis.Game.Database;
using fluXis.Game.Database.Maps;
using fluXis.Game.Graphics;
using fluXis.Game.Graphics.Background;
using fluXis.Game.Graphics.Background.Cropped;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Import;
using fluXis.Game.Input;
using fluXis.Game.Integration;
using fluXis.Game.Localization;
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
using fluXis.Game.Overlay.Register;
using fluXis.Game.Overlay.Settings;
using fluXis.Game.Overlay.User;
using fluXis.Game.Plugins;
using fluXis.Game.Screens;
using fluXis.Game.Screens.Gameplay.HUD;
using fluXis.Game.Screens.Menu;
using fluXis.Game.Screens.Skin;
using fluXis.Game.Skinning;
using fluXis.Game.UI;
using fluXis.Game.UI.Tips;
using fluXis.Game.Updater;
using fluXis.Game.Utils;
using fluXis.Resources;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Configuration;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.IO.Stores;
using osu.Framework.Localisation;
using osu.Framework.Platform;
using osu.Framework.Utils;
using osuTK;

namespace fluXis.Game;

public partial class FluXisGameBase : osu.Framework.Game
{
    // Anything in this class is shared between the test browser and the game implementation.
    // It allows for caching global dependencies that should be accessible to tests, or changing
    // the screen scaling for all components including the test browser and framework overlays.

    private DependencyContainer dependencies;

    public Vector2 ContentSize => content.DrawSize;

    private Vector2 targetDrawSize => new(1920, 1080);
    private DrawSizePreservingFillContainer drawSizePreserver;
    private Bindable<float> uiScale;

    protected override Container<Drawable> Content => content;
    private Container content;
    private int exceptionCount;
    protected virtual int MaxExceptions => IsDebug ? 5 : 1;

    protected GlobalClock GlobalClock { get; private set; }
    protected GlobalCursorOverlay CursorOverlay { get; private set; }
    protected LoginOverlay LoginOverlay { get; private set; }
    protected ChatOverlay ChatOverlay { get; private set; }
    protected UserProfileOverlay ProfileOverlay { get; private set; }
    protected RegisterOverlay RegisterOverlay { get; private set; }
    protected NotificationManager NotificationManager { get; private set; }
    protected MusicPlayer MusicPlayer { get; private set; }
    protected GlobalBackground GlobalBackground { get; private set; }
    protected UISamples Samples { get; private set; }
    protected Fluxel Fluxel { get; private set; }
    protected FluXisConfig Config { get; private set; }
    protected MapStore MapStore { get; private set; }

    public SettingsMenu Settings { get; private set; }
    public Toolbar Toolbar { get; private set; }
    public FluXisScreenStack ScreenStack { get; private set; }
    public MenuScreen MenuScreen { get; protected set; }
    public Dashboard Dashboard { get; private set; }

    private FluXisRealm realm;
    private KeybindStore keybindStore;
    private LightController lightController;
    private SkinManager skinManager;
    private PluginManager pluginManager;
    private ImportManager importManager;
    private PreviewManager previewManager;

    protected Bindable<UserActivity> Activity { get; } = new();
    public Season CurrentSeason { get; private set; }

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
    private void load(Storage storage, FrameworkConfigManager frameworkConfig)
    {
        frameworkLocale = frameworkConfig.GetBindable<string>(FrameworkSetting.Locale);
        frameworkLocale.BindValueChanged(_ => updateLanguage());

        localisationParameters = Localisation.CurrentParameters.GetBoundCopy();
        localisationParameters.BindValueChanged(_ => updateLanguage(), true);

        CurrentLanguage.BindValueChanged(val => frameworkLocale.Value = val.NewValue.ToCultureCode());

        Resources.AddExtension("json");
        Resources.AddStore(new DllResourceStore(FluXisResources.ResourceAssembly));
        initFonts();

        MapFiles.Initialize(storage.GetStorageForDirectory("maps"));

        CurrentSeason = getSeason();

        var endpoint = getApiEndpoint();

        dependencies.CacheAs(this);
        dependencies.CacheAs(Config = new FluXisConfig(storage));
        uiScale = Config.GetBindable<float>(FluXisSetting.UIScale);

        dependencies.Cache(GlobalClock = new GlobalClock());
        dependencies.Cache(realm = new FluXisRealm(storage));
        dependencies.Cache(NotificationManager = new NotificationManager());

        Fluxel = new Fluxel(endpoint);
        LoadComponent(Fluxel);
        dependencies.Cache(Fluxel);

        UserCache.Init(Fluxel);

        dependencies.Cache(new BackgroundTextureStore(Host, storage.GetStorageForDirectory("maps")));
        dependencies.Cache(new CroppedBackgroundStore(Host, storage.GetStorageForDirectory("maps")));
        dependencies.Cache(new OnlineTextureStore(Host, endpoint));

        LoadComponent(MapStore = new MapStore());
        dependencies.Cache(MapStore);

        LoadComponent(pluginManager = new PluginManager());
        dependencies.Cache(pluginManager);

        LoadComponent(importManager = new ImportManager());
        dependencies.Cache(importManager);

        LoadComponent(Samples = new UISamples());
        dependencies.Cache(Samples);

        dependencies.Cache(GlobalBackground = new GlobalBackground());
        dependencies.Cache(CursorOverlay = new GlobalCursorOverlay());
        dependencies.Cache(Settings = new SettingsMenu());
        dependencies.Cache(LoginOverlay = new LoginOverlay());
        dependencies.Cache(ChatOverlay = new ChatOverlay());
        dependencies.CacheAs(RegisterOverlay = new RegisterOverlay());
        dependencies.Cache(Toolbar = new Toolbar());
        dependencies.Cache(ScreenStack = new FluXisScreenStack { Activity = Activity });
        dependencies.Cache(ProfileOverlay = new UserProfileOverlay());
        dependencies.CacheAs(lightController = CreateLightController());
        dependencies.Cache(skinManager = new SkinManager());
        dependencies.Cache(MusicPlayer = new MusicPlayer { ScreenStack = ScreenStack });
        dependencies.Cache(Dashboard = new Dashboard());

        previewManager = new PreviewManager();
        LoadComponent(previewManager);
        dependencies.Cache(previewManager);

        var layoutManager = new LayoutManager();
        LoadComponent(layoutManager);
        dependencies.Cache(layoutManager);

        Textures.AddTextureSource(Host.CreateTextureLoaderStore(new HttpOnlineStore()));

        GlobalKeybindContainer keybinds;

        base.Content.Add(new SafeAreaContainer
        {
            RelativeSizeAxes = Axes.Both,
            Child = drawSizePreserver = new DrawSizePreservingFillContainer
            {
                TargetDrawSize = targetDrawSize,
                RelativeSizeAxes = Axes.Both,
                Children = new Drawable[]
                {
                    keybinds = new GlobalKeybindContainer(this, realm)
                    {
                        Children = new Drawable[]
                        {
                            importManager,
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
        keybindStore.AssignDefaults(new GameplayKeybindContainer(realm, 0));

        dependencies.Cache(keybinds);
        MenuSplashes.Load(storage);
        LoadingTips.Load(storage);
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        uiScale.BindValueChanged(e =>
        {
            var size = targetDrawSize / e.NewValue;
            drawSizePreserver.TargetDrawSize = size;
        }, true);
    }

    public void PerformUpdateCheck(bool silent, bool forceUpdate = false) => Task.Run(() => CreateUpdateManager()?.Perform(silent, forceUpdate));

    private Season getSeason()
    {
        var date = DateTime.Now;

        return date switch
        {
            { Month: 7 } or { Month: 8 } or { Month: 9 } => Season.Summer,
            { Month: 10, Day: >= 18 } => Season.Halloween,
            { Month: 12 } or { Month: 1 } => Season.Winter,
            _ => Season.Normal
        };
    }

    private APIEndpointConfig getApiEndpoint()
    {
        var path = Host.Storage.GetFullPath("server.json");

        if (File.Exists(path))
        {
            var json = File.ReadAllText(path);
            return json.Deserialize<APIEndpointConfig>().AddDefaults();
        }

        var defaultEndpoint = new APIEndpointConfig().AddDefaults();
        File.WriteAllText(path, defaultEndpoint.ToString());

        return getApiEndpoint();
    }

    private void initFonts()
    {
        AddFont(Resources, "Fonts/Renogare/Renogare");
        AddFont(Resources, "Fonts/RenogareSoft/RenogareSoft");
        AddFont(Resources, "Fonts/YoureGone/YoureGone");

        AddFont(Resources, "Fonts/Noto/Noto-Basic");
        AddFont(Resources, "Fonts/Noto/Noto-Hangul");
        AddFont(Resources, "Fonts/Noto/Noto-CJK-Basic");
        AddFont(Resources, "Fonts/Noto/Noto-CJK-Compatibility");
        AddFont(Resources, "Fonts/Noto/Noto-Thai");

        AddFont(Resources, "Fonts/FontAwesome6/FontAwesome6-Solid");
        AddFont(Resources, "Fonts/FontAwesome6/FontAwesome6-Regular");
        AddFont(Resources, "Fonts/FontAwesome6/FontAwesome6-Brands");
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

            NotificationManager.SendError("An unhandled error occurred!", e.Message, FontAwesome6.Solid.Bomb);
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
            NotificationManager.SendText("You are already in the Skin editor.", "", FontAwesome6.Solid.Bomb);
            return;
        }

        if (skinManager.IsDefault) return;

        Settings.Hide();
        ScreenStack.Push(new SkinEditor());
    }

    public void NextSong() => changeSong(1);
    public void PreviousSong() => changeSong(-1);

    private void changeSong(int change)
    {
        if (MapStore.MapSets.Count <= 0)
            return;

        var index = MapStore.CurrentMap?.Hash == "dummy"
            ? RNG.Next(0, MapStore.MapSets.Count)
            : MapStore.MapSets.IndexOf(MapStore.CurrentMapSet);

        index += change;

        if (index >= MapStore.MapSets.Count)
            index = 0;
        else if (index < 0)
            index = MapStore.MapSets.Count - 1;

        RealmMapSet set = MapStore.MapSets[index];
        SelectMapSet(set);
    }

    public void SelectMapSet(RealmMapSet set)
    {
        MapStore.CurrentMapSet = set;
        var map = set.Maps.First();
        GlobalBackground.AddBackgroundFromMap(map);
    }

    protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent) => dependencies = new DependencyContainer(base.CreateChildDependencies(parent));

    #region Localization

    public Bindable<Language> CurrentLanguage { get; } = new();

    public IEnumerable<Language> SupportedLanguages
    {
        get
        {
            var languages = Enum.GetValues(typeof(Language)).Cast<Language>().ToList();

            if (!IsDebug)
            {
                languages.Remove(Language.testing);
                languages.Remove(Language.debug);
            }

            return languages;
        }
    }

    private Bindable<string> frameworkLocale;
    private IBindable<LocalisationParameters> localisationParameters;

    private void updateLanguage()
    {
        CurrentLanguage.Value = LocaleUtils.GetLanguageFor(frameworkLocale.Value, localisationParameters.Value);
    }

    #endregion
}
