using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using fluXis.Audio;
using fluXis.Audio.Preview;
using fluXis.Configuration;
using fluXis.Configuration.Experiments;
using fluXis.Database;
using fluXis.Database.Maps;
using fluXis.Graphics;
using fluXis.Graphics.Background;
using fluXis.Graphics.Background.Cropped;
using fluXis.Graphics.Sprites;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Import;
using fluXis.Input;
using fluXis.Integration;
using fluXis.IO;
using fluXis.Localization;
using fluXis.Map;
using fluXis.Online;
using fluXis.Online.API.Models.Users;
using fluXis.Online.Chat;
using fluXis.Online.Fluxel;
using fluXis.Overlay.Mouse;
using fluXis.Overlay.Notifications;
using fluXis.Plugins;
using fluXis.Resources;
using fluXis.Scoring;
using fluXis.Screens.Gameplay.HUD;
using fluXis.Screens.Menu;
using fluXis.Skinning;
using fluXis.UI;
using fluXis.UI.Tips;
using fluXis.Utils;
using fluXis.Utils.Exceptions;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Configuration;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input;
using osu.Framework.IO.Stores;
using osu.Framework.Localisation;
using osu.Framework.Logging;
using osu.Framework.Platform;
using osu.Framework.Utils;
using osuTK;

namespace fluXis;

public partial class FluXisGameBase : osu.Framework.Game
{
    protected DependencyContainer GameDependencies { get; private set; }

    public Vector2 ContentSize => content.DrawSize;

    protected bool LoadFailed { get; set; }

    protected virtual bool LoadComponentsLazy => false;
    protected virtual bool RequiresSteam => false;

    protected LoadInfo LoadQueue { get; } = new();

    private Vector2 targetDrawSize => new(1920, 1080);
    private DrawSizePreservingFillContainer drawSizePreserver;
    private Bindable<float> uiScale;

    protected override Container<Drawable> Content => content;
    private Container content;
    private int exceptionCount;
    protected virtual int MaxExceptions => IsDebug ? 5 : 1;

    protected FluXisRealm Realm { get; private set; }
    protected FluXisConfig Config { get; private set; }
    protected NotificationManager NotificationManager { get; private set; }
    protected IAPIClient APIClient { get; private set; }
    protected MapStore MapStore { get; private set; }
    protected SkinManager SkinManager { get; private set; }
    protected LayoutManager LayoutManager { get; private set; }
    protected GlobalCursorOverlay CursorOverlay { get; private set; }

    [CanBeNull]
    protected ISteamManager Steam { get; }

    public PluginManager Plugins { get; private set; }
    public MenuScreen MenuScreen { get; protected set; }

    private KeybindStore keybindStore;
    private ImportManager importManager;

    private Storage exportStorage;
    public Storage ExportStorage => exportStorage ??= Host.Storage.GetStorageForDirectory("export");

    public Season CurrentSeason { get; private set; }

    public static string VersionString
    {
        get
        {
            var v = Version;

            if (v == null)
                return "unknown version";

            if (IsDebug)
                return "local development build";

            var str = $"v{v.Major}.{v.Minor}.{v.Build}";

            if (v.Revision > 0)
                str += $".{v.Revision}";

            return str;
        }
    }

    public static Version Version => Assembly.GetEntryAssembly()?.GetName().Version;
    public static bool IsDebug => Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyConfigurationAttribute>()?.Configuration == "Debug";

    public string ClientHash { get; private set; }

    public virtual LightController CreateLightController() => new();

    protected FluXisGameBase()
    {
        Midori.Logging.Logger.SaveToFiles = false;
        Steam = CreateSteam(); // steam needs to load before the graphics
    }

    [BackgroundDependencyLoader]
    private void load(Storage storage, FrameworkConfigManager frameworkConfig)
    {
        try
        {
            try
            {
                using var fs = File.OpenRead(typeof(FluXisGameBase).Assembly.Location);
                ClientHash = MapUtils.GetHash(fs);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Failed to get client hash!");
                ClientHash = MapUtils.GetHash(VersionString);
            }

            frameworkLocale = frameworkConfig.GetBindable<string>(FrameworkSetting.Locale);
            frameworkLocale.BindValueChanged(_ => updateLanguage());

            localisationParameters = Localisation.CurrentParameters.GetBoundCopy();
            localisationParameters.BindValueChanged(_ => updateLanguage(), true);

            CurrentLanguage.BindValueChanged(val => frameworkLocale.Value = val.NewValue.ToCultureCode());

            Resources.AddExtension("json");
            Resources.AddExtension("ogg");
            Resources.AddStore(new DllResourceStore(FluXisResources.ResourceAssembly));
            initFonts();

            if (RequiresSteam && !(Steam?.Initialized ?? false))
            {
                Host.OpenUrlExternally("https://store.steampowered.com/app/3440100");
                throw new SteamInitException();
            }

            CurrentSeason = getSeason();

            var endpoint = getApiEndpoint();

            cacheComponent(this);

            var mapStorage = storage.GetStorageForDirectory("maps");
            MapFiles.Initialize(mapStorage);

            Realm = new FluXisRealm(storage);
            cacheComponent(Realm);

            cacheComponent(Config = new FluXisConfig(storage));
            cacheComponent(new ExperimentConfigManager(storage));
            uiScale = Config.GetBindable<float>(FluXisSetting.UIScale);

            cacheComponent(NotificationManager = new NotificationManager());

            cacheComponent(APIClient = new FluxelClient(endpoint), true, true);
            cacheComponent(APIClient as FluxelClient);
            cacheComponent(new ChatClient(), true, true);

            var users = new UserCache();
            cacheComponent(users, true, true);

            cacheComponent(new BackgroundTextureStore(Host, mapStorage));
            cacheComponent(new CroppedBackgroundStore(Host, mapStorage));
            cacheComponent(new OnlineTextureStore(Host, endpoint));

            cacheComponent(MapStore = new MapStore(), true, true);
            cacheComponent(new ScoreManager(), true, true);
            cacheComponent(new ReplayStorage(storage.GetStorageForDirectory("replays")));

            cacheComponent(Plugins = new PluginManager(), true);
            cacheComponent(importManager = new ImportManager(), true, true);
            cacheComponent(SkinManager = new SkinManager(), true, true);
            cacheComponent<ISkin>(SkinManager);
            cacheComponent(LayoutManager = new LayoutManager(), true);
            cacheComponent(new PreviewManager(), true);

            cacheComponent(new UISamples(), true, true);
            cacheComponent(CreateLightController(), true, true);
            cacheComponent(CursorOverlay = new GlobalCursorOverlay());

            if (Steam is not null)
                cacheComponent(Steam, true, true);

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
                        keybinds = new GlobalKeybindContainer(this, Realm)
                        {
                            Child = CursorOverlay.WithChild(content = new GlobalTooltipContainer(CursorOverlay.Cursor)
                            {
                                RelativeSizeAxes = Axes.Both
                            })
                        },
                        new GamepadHandler()
                    }
                }
            });

            keybindStore = new KeybindStore(Realm);
            keybindStore.AssignDefaults(keybinds);
            keybindStore.AssignDefaults(new GameplayKeybindContainer(Realm, 0, true));

            cacheComponent(keybinds);
            MenuSplashes.Load(Host.CacheStorage);
            LoadingTips.Load(Host.CacheStorage);
        }
        catch (Exception ex)
        {
            LoadFailed = true;
            Logger.Error(ex, "Failed to initialize game!");

            base.Content.Child = new FillFlowContainer
            {
                AutoSizeAxes = Axes.Both,
                Direction = FillDirection.Vertical,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Children = new Drawable[]
                {
                    new FluXisSpriteText
                    {
                        Text = "Failed to initialize game!",
                        WebFontSize = 32,
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre
                    },
                    new FluXisSpriteText
                    {
                        Text = $"{ex.GetType().Name}: {ex.Message}",
                        WebFontSize = 16,
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        Colour = FluXisColors.Red
                    }
                }
            };
        }
    }

    private void cacheComponent<T>(T component, bool load = false, bool add = false)
        where T : class
    {
        var drawable = component as Drawable;

        if (load && drawable == null)
            throw new InvalidOperationException($"Component of type {typeof(T)} is not a drawable.");

        GameDependencies.CacheAs(component);

        if (!load || drawable.IsLoaded)
            return;

        if (LoadComponentsLazy)
        {
            CreateComponentLoadTask(drawable, _ =>
            {
                if (add)
                    base.Content.Add(drawable);
            });

            return;
        }

        Schedule(() =>
        {
            Logger.Log($"Loading {component.GetType().Name}...", LoggingTarget.Runtime, LogLevel.Debug);

            if (!drawable.IsLoaded)
                LoadComponent(drawable);

            if (add)
                base.Content.Add(drawable);
        });
    }

    protected void CreateComponentLoadTask<T>(T component, Action<T> action)
        where T : Drawable
    {
        var name = component.GetType().Name;

        var task = new LoadTask($"Loading {name}...", complete =>
        {
            var sw = new Stopwatch();
            sw.Start();

            Logger.Log($"Loading {name}...", LoggingTarget.Runtime, LogLevel.Debug);

            LoadComponentAsync(component, c =>
            {
                action?.Invoke(c);
                complete.Invoke();

                sw.Stop();
                Logger.Log($"Finished loading {name} in {sw.ElapsedMilliseconds}ms.", LoggingTarget.Runtime, LogLevel.Debug);
            });
        });

        LoadQueue.Push(task);
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

    private Season getSeason()
    {
        var date = DateTime.Now;

        return date switch
        {
            { Month: 7 } or { Month: 8 } or { Month: 9, Day: <= 7 } => Season.Summer,
            { Month: 10 } => Season.Halloween,
            { Month: 12 } => Season.Christmas,
            { Month: 1 } => Season.Winter,
            _ => Season.Normal
        };
    }

    private APIEndpointConfig getApiEndpoint()
    {
#if CLOSED_TESTING
        return new APIEndpointConfig().AddLocalDefaults();
#else
        var path = Host.Storage.GetFullPath("server.json");

        if (File.Exists(path))
        {
            var json = File.ReadAllText(path);
            return json.Deserialize<APIEndpointConfig>().AddDefaults();
        }

        var defaultEndpoint = new APIEndpointConfig().AddDefaults();
        File.WriteAllText(path, defaultEndpoint.Serialize());

        return getApiEndpoint();
#endif
    }

    private void initFonts()
    {
        // Resharper disable StringLiteralTypo
        AddFont(Resources, "Fonts/Renogare/Renogare");
        AddFont(Resources, "Fonts/RenogareSoft/RenogareSoft");
        AddFont(Resources, "Fonts/YoureGone/YoureGone");

        AddFont(Resources, "Fonts/MOBO/MOBO");

        AddFont(Resources, "Fonts/Noto/Noto-Basic");
        AddFont(Resources, "Fonts/Noto/Noto-Hangul");
        AddFont(Resources, "Fonts/Noto/Noto-CJK-Basic");
        AddFont(Resources, "Fonts/Noto/Noto-CJK-Compatibility");
        AddFont(Resources, "Fonts/Noto/Noto-Thai");

        AddFont(Resources, "Fonts/FontAwesome6/FontAwesome6-Solid");
        AddFont(Resources, "Fonts/FontAwesome6/FontAwesome6-Regular");
        AddFont(Resources, "Fonts/FontAwesome6/FontAwesome6-Brands");

        AddFont(Resources, "Fonts/JetBrainsMono/JetBrainsMono");
        // Resharper restore StringLiteralTypo
    }

    public new virtual void Exit()
    {
        APIClient.Disconnect();
        base.Exit();
    }

    [CanBeNull]
    protected virtual ISteamManager CreateSteam() => null;

    public override void SetHost(GameHost host)
    {
        base.SetHost(host);

        host.ExceptionThrown += e =>
        {
            exceptionCount++;
            Task.Delay(1000).ContinueWith(_ => exceptionCount--);

            NotificationManager?.SendError("An unhandled error occurred!", IsDebug ? e.Message : "This has been automatically reported to the developers.", FontAwesome6.Solid.Bomb);
            return exceptionCount <= MaxExceptions;
        };
    }

    protected override IDictionary<FrameworkSetting, object> GetFrameworkConfigDefaults() => new Dictionary<FrameworkSetting, object>
    {
        { FrameworkSetting.VolumeUniversal, .15d },
        { FrameworkSetting.ConfineMouseMode, ConfineMouseMode.Never },
    };

    protected override bool OnExiting()
    {
        APIClient.Disconnect();
        return base.OnExiting();
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

    public virtual void SelectMapSet(RealmMapSet set)
    {
        MapStore.CurrentMapSet = set;
    }

    public virtual void CloseOverlays() { }
    public virtual void PresentScore(RealmMap map, ScoreInfo score, APIUser player, Action replayAction = null) { }
    public virtual void ShowMap(RealmMapSet map) { }

    protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent) => GameDependencies = new DependencyContainer(base.CreateChildDependencies(parent));

    #region Localization

    public Bindable<Language> CurrentLanguage { get; } = new();

    public virtual IEnumerable<Language> SupportedLanguages
    {
        get
        {
            var languages = Enum.GetValues<Language>().ToList();

            if (!IsDebug)
                languages.Remove(Language.debug);

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

    public class LoadInfo
    {
        private Queue<LoadTask> queue { get; } = new();

        public long TasksFinished { get; private set; }
        public long TasksTotal { get; private set; }

        public event Action<LoadTask> TaskStarted;
        public event Action AllFinished;

        public void Push(LoadTask task)
        {
            queue.Enqueue(task);
            TasksTotal = queue.Count;
        }

        public LoadTask PerformNext(Action then)
        {
            if (queue.Count == 0)
            {
                AllFinished?.Invoke();
                return null;
            }

            var task = queue.Dequeue();
            TaskStarted?.Invoke(task);
            task.Perform?.Invoke(() =>
            {
                TasksFinished++;
                then?.Invoke();
            });

            return task;
        }

        public override string ToString()
        {
            var finished = TasksTotal - queue.Count;
            return $"{finished / TasksTotal * 100:00.00}% ({finished}/{TasksTotal})";
        }
    }

    public class LoadTask
    {
        public string Name { get; }
        public Action<Action> Perform { get; }

        public LoadTask(string name, Action<Action> perform)
        {
            Name = name;
            Perform = perform;
        }

        public override string ToString() => Name;
    }
}
