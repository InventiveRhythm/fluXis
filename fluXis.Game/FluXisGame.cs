using System;
using System.IO;
using System.Linq;
using fluXis.Game.Audio;
using fluXis.Game.Configuration;
using fluXis.Game.Database.Maps;
using fluXis.Game.Graphics.Background;
using fluXis.Game.Graphics.UserInterface.Panel;
using fluXis.Game.Input;
using fluXis.Game.Localization;
using fluXis.Game.Localization.Stores;
using fluXis.Game.Online.API.Models.Other;
using fluXis.Game.Online.API.Models.Users;
using fluXis.Game.Online.Fluxel;
using fluXis.Game.Overlay.Achievements;
using fluXis.Game.Overlay.Chat;
using fluXis.Game.Overlay.Exit;
using fluXis.Game.Overlay.FPS;
using fluXis.Game.Overlay.Login;
using fluXis.Game.Overlay.Mouse;
using fluXis.Game.Overlay.Music;
using fluXis.Game.Overlay.Network;
using fluXis.Game.Overlay.Notifications;
using fluXis.Game.Overlay.Notifications.Tasks;
using fluXis.Game.Overlay.Notifications.Types.Image;
using fluXis.Game.Overlay.Register;
using fluXis.Game.Overlay.Settings;
using fluXis.Game.Overlay.Toolbar;
using fluXis.Game.Overlay.User;
using fluXis.Game.Overlay.Volume;
using fluXis.Game.Scoring;
using fluXis.Game.Screens;
using fluXis.Game.Screens.Menu;
using fluXis.Game.Screens.Result;
using fluXis.Game.Screens.Select;
using fluXis.Game.Screens.Skin;
using fluXis.Game.Screens.Warning;
using fluXis.Game.Utils;
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

namespace fluXis.Game;

public partial class FluXisGame : FluXisGameBase, IKeyBindingHandler<FluXisGlobalKeybind>
{
    public static readonly string[] AUDIO_EXTENSIONS = { ".mp3", ".wav", ".ogg" };
    public static readonly string[] IMAGE_EXTENSIONS = { ".jpg", ".jpeg", ".png" };
    public static readonly string[] VIDEO_EXTENSIONS = { ".mp4", ".mov", ".avi", ".flv", ".mpg", ".wmv", ".m4v" };

    private BufferedContainer buffer;
    private GlobalClock globalClock;
    private GlobalBackground globalBackground;
    private Container screenContainer;
    private FluXisScreenStack screenStack;
    private Container<VisibilityContainer> overlayContainer;
    private Toolbar toolbar;
    private PanelContainer panelContainer;
    private FloatingNotificationContainer notificationContainer;
    private ExitAnimation exitAnimation;

    private readonly BindableDouble inactiveVolume = new(1f);

    [UsedImplicitly]
    public bool Sex { get; private set; }

    [BackgroundDependencyLoader]
    private void load()
    {
        GameDependencies.CacheAs(this);

        Children = new Drawable[]
        {
            buffer = new BufferedContainer
            {
                RelativeSizeAxes = Axes.Both,
                RedrawOnScale = false
            }
        };

        loadComponent(globalClock = new GlobalClock(), Add, true);
        loadComponent(NotificationManager, Add);

        loadComponent(globalBackground = new GlobalBackground(), buffer.Add, true);
        loadComponent(screenContainer = new Container { RelativeSizeAxes = Axes.Both }, buffer.Add);
        loadComponent(screenStack = new FluXisScreenStack(Activity), screenContainer.Add, true);

        loadComponent(overlayContainer = new Container<VisibilityContainer> { RelativeSizeAxes = Axes.Both }, buffer.Add);
        loadComponent(new Dashboard(), overlayContainer.Add, true);
        loadComponent(new ChatOverlay(), overlayContainer.Add, true);
        loadComponent(new UserProfileOverlay(), overlayContainer.Add, true);
        loadComponent(new MusicPlayer(), overlayContainer.Add, true);
        loadComponent(new SettingsMenu(), overlayContainer.Add, true);

        loadComponent(new LoginOverlay(), buffer.Add, true);
        loadComponent(new RegisterOverlay(), buffer.Add, true);
        loadComponent(toolbar = new Toolbar(), buffer.Add, true);

        loadComponent(panelContainer = new PanelContainer { BlurContainer = buffer }, Add, true);
        loadComponent(new VolumeOverlay(), Add);

        loadComponent(notificationContainer = new FloatingNotificationContainer(), fnc =>
        {
            Add(fnc);
            NotificationManager.Floating = fnc;
        });

        loadComponent(new TaskNotificationContainer(), tnc =>
        {
            Add(tnc);
            NotificationManager.Tasks = tnc;
        });

        loadComponent(new FpsOverlay(), Add);
        loadComponent(new GlobalCursorOverlay(), Add);
        loadComponent(exitAnimation = new ExitAnimation(), Add);

        Audio.AddAdjustment(AdjustableProperty.Volume, inactiveVolume);

        IsActive.BindValueChanged(active =>
        {
            var volume = Config.Get<double>(FluXisSetting.InactiveVolume);
            this.TransformBindableTo(inactiveVolume, active.NewValue ? 1 : volume, 1000, Easing.OutQuint);
        }, true);
    }

    private T loadComponent<T>(T component, Action<T> action, bool cache = false, bool preload = false)
        where T : Drawable
    {
        if (cache)
            GameDependencies.CacheAs(component);

        if (preload)
        {
            AddInternal(component);
            action(component);
            return component;
        }

        Schedule(() =>
        {
            Logger.Log($"Loading {component.GetType().Name}...", LoggingTarget.Runtime, LogLevel.Debug);

            LoadComponent(component);
            action(component);
        });

        return component;
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        PerformUpdateCheck(true);

        loadLocales();

        ScheduleAfterChildren(() =>
        {
            screenStack.Push(new WarningScreen());
            MenuScreen = new MenuScreen();
            LoadComponent(MenuScreen);
        });

        Fluxel.RegisterListener<Achievement>(EventType.Achievement, res =>
        {
            var achievement = res.Data;
            Schedule(() => panelContainer.Content = new AchievementOverlay(achievement));
        });

        Fluxel.RegisterListener<ServerMessage>(EventType.ServerMessage, res =>
        {
            var data = res.Data;

            switch (data.Type)
            {
                case "normal":
                    NotificationManager.SendText(data.Text, data.SubText);
                    break;

                case "small":
                    NotificationManager.SendSmallText(data.Text);
                    break;

                case "image":
                    NotificationManager.Add(new ImageNotificationData
                    {
                        Text = data.Text,
                        Path = data.Path,
                        Location = ImageNotificationData.ImageLocation.Online
                    });
                    break;
            }
        });

        if (!Config.Get<bool>(FluXisSetting.NowPlaying)) return;

        MapStore.MapSetBindable.BindValueChanged(_ =>
        {
            var song = MapStore.CurrentMapSet;
            if (song == null) return;

            var data = new
            {
                player = "fluXis",
                title = song.Metadata.Title,
                artist = song.Metadata.Artist,
                cover = $"{Fluxel.Endpoint.AssetUrl}/cover/{song.OnlineID}",
                background = $"{Fluxel.Endpoint.AssetUrl}/background/{song.OnlineID}",
            };

            File.WriteAllText($"{Host.Storage.GetFullPath("nowplaying.json")}", data.Serialize());
        });
    }

    public override void SelectMapSet(RealmMapSet set)
    {
        base.SelectMapSet(set);
        globalBackground.AddBackgroundFromMap(set.Maps.First());
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

    public override void CloseOverlays() => overlayContainer.Children.ForEach(c => c.Hide());

    public override void PresentScore(RealmMap map, ScoreInfo score, APIUserShort player)
    {
        if (map == null || score == null)
            throw new ArgumentNullException();

        screenStack.Push(new SoloResults(map, score, player));
    }

    public override void ShowMap(RealmMapSet set)
    {
        CloseOverlays();
        SelectMapSet(set);

        if (screenStack.CurrentScreen is not SelectScreen)
        {
            MenuScreen.MakeCurrent();
            MenuScreen.Push(new SelectScreen());
        }
    }

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

        if (screenStack.AllowMusicControl)
        {
            switch (e.Action)
            {
                case FluXisGlobalKeybind.MusicPause:
                    if (globalClock.IsRunning) globalClock.Stop();
                    else globalClock.Start();
                    return true;

                case FluXisGlobalKeybind.MusicPrevious:
                    PreviousSong();
                    return true;

                case FluXisGlobalKeybind.MusicNext:
                    NextSong();
                    return true;
            }
        }

        return false;
    }

    public void OnReleased(KeyBindingReleaseEvent<FluXisGlobalKeybind> e) { }

    protected override void Update()
    {
        screenContainer.Padding = new MarginPadding { Top = toolbar.Height + toolbar.Y };
        notificationContainer.Y = toolbar.Height + toolbar.Y;

        if (globalClock.Finished && screenStack.CurrentScreen is FluXisScreen { AutoPlayNext: true })
            NextSong();
    }

    public override void Exit()
    {
        toolbar.ShowToolbar.Value = false;
        globalClock.FadeOut(1500);
        exitAnimation.Show(buffer.Hide, base.Exit);
    }

    private void loadLocales()
    {
        var localeStore = new NamespacedResourceStore<byte[]>(Resources, "Localization");
        localeStore.AddExtension("json");

        var languages = Enum.GetValues<Language>();

        var mappings = languages.Select(l =>
        {
            if (l == Language.testing) return new LocaleMapping("testing", new TestingLocaleStore("en", localeStore));
            if (l == Language.debug) return new LocaleMapping("debug", new DebugLocaleStore());

            var code = l.ToCultureCode();

            try
            {
                return new LocaleMapping(code, new ResourceLocaleStore(code, localeStore));
            }
            catch (FileNotFoundException)
            {
                return null;
            }
        }).Where(m => m != null);

        Localisation.AddLocaleMappings(mappings);
    }
}
