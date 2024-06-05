using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using fluXis.Game.Audio;
using fluXis.Game.Audio.Preview;
using fluXis.Game.Configuration;
using fluXis.Game.Configuration.Experiments;
using fluXis.Game.Database;
using fluXis.Game.Database.Maps;
using fluXis.Game.Graphics;
using fluXis.Game.Graphics.Background;
using fluXis.Game.Graphics.Background.Cropped;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Import;
using fluXis.Game.Input;
using fluXis.Game.Integration;
using fluXis.Game.IO;
using fluXis.Game.Localization;
using fluXis.Game.Map;
using fluXis.Game.Online;
using fluXis.Game.Online.Activity;
using fluXis.Game.Online.API.Models.Chat;
using fluXis.Game.Online.API.Models.Groups;
using fluXis.Game.Online.API.Models.Maps;
using fluXis.Game.Online.API.Models.Multi;
using fluXis.Game.Online.API.Models.Users;
using fluXis.Game.Online.Fluxel;
using fluXis.Game.Online.Multiplayer;
using fluXis.Game.Overlay.Mouse;
using fluXis.Game.Overlay.Notifications;
using fluXis.Game.Plugins;
using fluXis.Game.Screens.Edit.Input;
using fluXis.Game.Screens.Gameplay.HUD;
using fluXis.Game.Screens.Menu;
using fluXis.Game.Skinning;
using fluXis.Game.UI;
using fluXis.Game.UI.Tips;
using fluXis.Game.Updater;
using fluXis.Game.Utils;
using fluXis.Resources;
using fluXis.Shared.Components.Chat;
using fluXis.Shared.Components.Groups;
using fluXis.Shared.Components.Maps;
using fluXis.Shared.Components.Multi;
using fluXis.Shared.Components.Users;
using fluXis.Shared.Scoring;
using fluXis.Shared.Utils;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Configuration;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.IO.Stores;
using osu.Framework.Localisation;
using osu.Framework.Logging;
using osu.Framework.Platform;
using osu.Framework.Utils;
using osuTK;

namespace fluXis.Game;

public partial class FluXisGameBase : osu.Framework.Game
{
    protected DependencyContainer GameDependencies { get; private set; }

    public Vector2 ContentSize => content.DrawSize;

    protected virtual bool LoadComponentsLazy => false;
    protected Dictionary<Drawable, Action<Drawable>> LoadQueue { get; } = new();

    private Vector2 targetDrawSize => new(1920, 1080);
    private DrawSizePreservingFillContainer drawSizePreserver;
    private Bindable<float> uiScale;

    protected override Container<Drawable> Content => content;
    private Container content;
    private int exceptionCount;
    protected virtual int MaxExceptions => IsDebug ? 5 : 1;

    protected FluXisConfig Config { get; private set; }
    protected NotificationManager NotificationManager { get; private set; }
    protected FluxelClient Fluxel { get; private set; }
    protected MapStore MapStore { get; private set; }
    protected SkinManager SkinManager { get; private set; }
    protected GlobalCursorOverlay CursorOverlay { get; private set; }

    public MenuScreen MenuScreen { get; protected set; }

    private KeybindStore keybindStore;
    private ImportManager importManager;

    protected Bindable<UserActivity> Activity { get; } = new();
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

    public virtual LightController CreateLightController() => new();
    public virtual IUpdatePerformer CreateUpdatePerformer() => null;

    protected FluXisGameBase()
    {
        JsonUtils.RegisterTypeConversion<IAPIUser, APIUser>();
        JsonUtils.RegisterTypeConversion<IAPIUserSocials, APIUser.APIUserSocials>();
        JsonUtils.RegisterTypeConversion<IMultiplayerParticipant, MultiplayerParticipant>();
        JsonUtils.RegisterTypeConversion<IAPIGroup, APIGroup>();
        JsonUtils.RegisterTypeConversion<IAPIMapShort, APIMapShort>();
        JsonUtils.RegisterTypeConversion<IMultiplayerRoom, MultiplayerRoom>();
        JsonUtils.RegisterTypeConversion<IMultiplayerRoomSettings, MultiplayerRoomSettings>();
        JsonUtils.RegisterTypeConversion<IChatMessage, ChatMessage>();
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

        CurrentSeason = getSeason();

        var endpoint = getApiEndpoint();

        cacheComponent(this);

        var mapStorage = storage.GetStorageForDirectory("maps");
        MapFiles.Initialize(mapStorage);

        var realm = new FluXisRealm(storage);
        cacheComponent(realm);

        cacheComponent(Config = new FluXisConfig(storage));
        cacheComponent(new ExperimentConfigManager(storage));
        uiScale = Config.GetBindable<float>(FluXisSetting.UIScale);

        cacheComponent(NotificationManager = new NotificationManager());

        cacheComponent(Fluxel = new FluxelClient(endpoint), true, true);
        cacheComponent<IAPIClient>(Fluxel);
        cacheComponent<MultiplayerClient>(new OnlineMultiplayerClient(), true, true);

        UserCache.Init(Fluxel);

        cacheComponent(new BackgroundTextureStore(Host, mapStorage));
        cacheComponent(new CroppedBackgroundStore(Host, mapStorage));
        cacheComponent(new OnlineTextureStore(Host, endpoint));

        cacheComponent(MapStore = new MapStore(), true);
        cacheComponent(new ReplayStorage(storage.GetStorageForDirectory("replays")));

        cacheComponent(new PluginManager(), true);
        cacheComponent(importManager = new ImportManager(), true, true);
        cacheComponent(SkinManager = new SkinManager(), true, true);
        cacheComponent(new LayoutManager(), true);
        cacheComponent(new PreviewManager(), true);

        cacheComponent(new UISamples(), true, true);
        cacheComponent(CreateLightController(), true, true);
        cacheComponent(CursorOverlay = new GlobalCursorOverlay());

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
                        Child = CursorOverlay.WithChild(content = new GlobalTooltipContainer(CursorOverlay.Cursor)
                        {
                            RelativeSizeAxes = Axes.Both
                        })
                    },
                    new GamepadHandler()
                }
            }
        });

        keybindStore = new KeybindStore(realm);
        keybindStore.AssignDefaults(keybinds);
        keybindStore.AssignDefaults(new GameplayKeybindContainer(realm, 0));
        keybindStore.AssignDefaults(new EditorKeybindingContainer(null, realm));

        cacheComponent(keybinds);
        MenuSplashes.Load(Host.CacheStorage);
        LoadingTips.Load(Host.CacheStorage);
    }

    private void cacheComponent<T>(T component, bool load = false, bool add = false)
        where T : class
    {
        var drawable = component as Drawable;

        if (load && drawable == null)
            throw new InvalidOperationException($"Component of type {typeof(T)} is not a drawable.");

        GameDependencies.CacheAs(component);

        if (!load)
            return;

        if (LoadComponentsLazy)
        {
            LoadQueue[drawable] = d =>
            {
                if (add)
                    base.Content.Add(d);
            };

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

    protected override void LoadComplete()
    {
        base.LoadComplete();

        uiScale.BindValueChanged(e =>
        {
            var size = targetDrawSize / e.NewValue;
            drawSizePreserver.TargetDrawSize = size;
        }, true);
    }

    public void PerformUpdateCheck(bool silent, bool forceUpdate = false)
    {
        Task.Run(() =>
        {
            var checker = new UpdateChecker(Config.Get<ReleaseChannel>(FluXisSetting.ReleaseChannel));

            if (forceUpdate || checker.UpdateAvailable)
            {
                var performer = CreateUpdatePerformer();
                var version = checker.LatestVersion;

                if (performer != null)
                    performer.Perform(version);
                else
                    NotificationManager.SendText($"New update available! ({version})", "Check the github releases to download the latest version.", FontAwesome6.Solid.Download);
            }
            else if (!silent)
                NotificationManager.SendText("No updates available.", "You are running the latest version.", FontAwesome6.Solid.Check);
        });
    }

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
        File.WriteAllText(path, defaultEndpoint.Serialize());

        return getApiEndpoint();
    }

    private void initFonts()
    {
        // Resharper disable StringLiteralTypo
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
        // Resharper restore StringLiteralTypo
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

    protected override IDictionary<FrameworkSetting, object> GetFrameworkConfigDefaults() => new Dictionary<FrameworkSetting, object>
    {
        { FrameworkSetting.VolumeUniversal, .15d }
    };

    protected override bool OnExiting()
    {
        Fluxel.Close();
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
    public virtual void PresentScore(RealmMap map, ScoreInfo score, APIUserShort player) { }
    public virtual void ShowMap(RealmMapSet map) { }

    protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent) => GameDependencies = new DependencyContainer(base.CreateChildDependencies(parent));

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
