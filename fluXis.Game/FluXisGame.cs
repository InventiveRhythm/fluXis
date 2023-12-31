using System;
using System.IO;
using System.Linq;
using fluXis.Game.Audio;
using fluXis.Game.Configuration;
using fluXis.Game.Graphics.Containers;
using fluXis.Game.Input;
using fluXis.Game.Localization;
using fluXis.Game.Localization.Stores;
using fluXis.Game.Online.API.Models.Other;
using fluXis.Game.Online.Fluxel;
using fluXis.Game.Overlay.Exit;
using fluXis.Game.Overlay.FPS;
using fluXis.Game.Overlay.Notifications;
using fluXis.Game.Overlay.Notifications.Types.Image;
using fluXis.Game.Overlay.Volume;
using fluXis.Game.Screens;
using fluXis.Game.Screens.Menu;
using fluXis.Game.Screens.Warning;
using fluXis.Game.Utils;
using JetBrains.Annotations;
using Newtonsoft.Json;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.IO.Stores;
using osu.Framework.Localisation;
using osu.Framework.Screens;
using osuTK;

namespace fluXis.Game;

public partial class FluXisGame : FluXisGameBase, IKeyBindingHandler<FluXisGlobalKeybind>
{
    public static readonly string[] AUDIO_EXTENSIONS = { ".mp3", ".wav", ".ogg" };
    public static readonly string[] IMAGE_EXTENSIONS = { ".jpg", ".jpeg", ".png" };
    public static readonly string[] VIDEO_EXTENSIONS = { ".mp4", ".mov", ".avi", ".flv", ".mpg", ".wmv", ".m4v" };

    private Container screenContainer;
    private ExitAnimation exitAnimation;
    private Container overlayDim;
    private Container overlayContainer;

    private FloatingNotificationContainer notificationContainer;
    private BufferedContainer buffer;

    public override Drawable Overlay
    {
        get => overlayContainer.Count == 0 ? null : overlayContainer[0];
        set
        {
            overlayContainer.Clear();
            if (value != null) overlayContainer.Add(value);
        }
    }

    [UsedImplicitly]
    public bool Sex = true;

    [BackgroundDependencyLoader]
    private void load()
    {
        Children = new Drawable[]
        {
            Fluxel,
            GlobalClock,
            Samples,
            NotificationManager,
            buffer = new BufferedContainer
            {
                RelativeSizeAxes = Axes.Both,
                RedrawOnScale = false,
                Children = new Drawable[]
                {
                    BackgroundStack,
                    screenContainer = new Container
                    {
                        RelativeSizeAxes = Axes.Both,
                        Padding = new MarginPadding { Top = Toolbar.Height },
                        Children = new Drawable[]
                        {
                            ScreenStack
                        }
                    },
                    Dashboard,
                    MusicPlayer,
                    LoginOverlay,
                    Toolbar,
                    RegisterOverlay,
                    ChatOverlay,
                    ProfileOverlay,
                    Settings
                }
            },
            new PopoverContainer
            {
                RelativeSizeAxes = Axes.Both,
                Children = new Drawable[]
                {
                    overlayDim = new FullInputBlockingContainer
                    {
                        RelativeSizeAxes = Axes.Both,
                        Alpha = 0,
                        Child = new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Colour = Colour4.Black,
                            Alpha = .5f
                        }
                    },
                    overlayContainer = new Container
                    {
                        RelativeSizeAxes = Axes.Both
                    }
                }
            },
            new VolumeOverlay(),
            NotificationManager.Floating = notificationContainer = new FloatingNotificationContainer(),
            new FpsOverlay(),
            CursorOverlay,
            exitAnimation = new ExitAnimation()
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        PerformUpdateCheck(true);

        loadLocales();

        ScreenStack.Push(new WarningScreen());
        MenuScreen = new MenuScreen();
        LoadComponent(MenuScreen);

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

            var json = JsonConvert.SerializeObject(data);
            File.WriteAllText($"{Host.Storage.GetFullPath("nowplaying.json")}", json);
        });
    }

    public bool OnPressed(KeyBindingPressEvent<FluXisGlobalKeybind> e)
    {
        if (e.Repeat) return false;

        switch (e.Action)
        {
            case FluXisGlobalKeybind.ToggleSettings:
                Settings.ToggleVisibility();
                return true;

            case FluXisGlobalKeybind.ToggleDashboard when Fluxel.LoggedInUser != null:
                Dashboard.ToggleVisibility();
                return true;

            case FluXisGlobalKeybind.ToggleMusicPlayer:
                MusicPlayer.ToggleVisibility();
                return true;

            case FluXisGlobalKeybind.OpenSkinEditor:
                OpenSkinEditor();
                return true;

            case FluXisGlobalKeybind.Home:
                MenuScreen?.MakeCurrent(); // stupid to use it from Toolbar, but it works
                return true;
        }

        if (ScreenStack.AllowMusicControl)
        {
            switch (e.Action)
            {
                case FluXisGlobalKeybind.MusicPause:
                    if (GlobalClock.IsRunning) GlobalClock.Stop();
                    else GlobalClock.Start();
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
        screenContainer.Padding = new MarginPadding { Top = Toolbar.Height + Toolbar.Y };
        notificationContainer.Y = Toolbar.Height + Toolbar.Y;

        if (Overlay is { IsLoaded: true, IsPresent: false } && !Overlay.Transforms.Any())
        {
            Schedule(() => overlayContainer.Remove(Overlay, false));
            overlayDim.Alpha = 0;
            buffer.BlurSigma = Vector2.Zero;
            GlobalClock.LowPassFilter.Cutoff = LowPassFilter.MAX;
        }
        else if (Overlay is { IsLoaded: true })
        {
            overlayDim.Alpha = Overlay.Alpha;
            buffer.BlurSigma = new Vector2(Overlay.Alpha * 4);

            var lowpass = (LowPassFilter.MAX - LowPassFilter.MIN) * Overlay.Alpha;
            lowpass = LowPassFilter.MAX - lowpass;
            GlobalClock.LowPassFilter.Cutoff = (int)lowpass;
        }
        else if (buffer.BlurSigma != Vector2.Zero || overlayDim.Alpha != 0)
        {
            overlayDim.Alpha = 0;
            buffer.BlurSigma = new Vector2(0);
        }

        if (GlobalClock.Finished && ScreenStack.CurrentScreen is FluXisScreen { AutoPlayNext: true })
            NextSong();
    }

    public override void Exit()
    {
        CursorOverlay.FadeOut(600);
        Toolbar.ShowToolbar.Value = false;
        GlobalClock.FadeOut(1500);
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
