using System.Linq;
using fluXis.Game.Audio;
using fluXis.Game.Configuration;
using fluXis.Game.Graphics.Background;
using fluXis.Game.Graphics.Containers;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Panel;
using fluXis.Game.Graphics.UserInterface.Panel.Presets;
using fluXis.Game.Graphics.UserInterface.Text;
using fluXis.Game.Localization;
using fluXis.Game.Map;
using fluXis.Game.Online.Fluxel;
using fluXis.Game.Overlay.Auth;
using fluXis.Game.Overlay.Network;
using fluXis.Game.Overlay.Settings;
using fluXis.Game.Overlay.Toolbar;
using fluXis.Game.Screens.Browse;
using fluXis.Game.Screens.Edit;
using fluXis.Game.Screens.Menu.UI;
using fluXis.Game.Screens.Menu.UI.Buttons;
using fluXis.Game.Screens.Menu.UI.NowPlaying;
using fluXis.Game.Screens.Menu.UI.Snow;
using fluXis.Game.Screens.Menu.UI.Updates;
using fluXis.Game.Screens.Menu.UI.Visualizer;
using fluXis.Game.Screens.Multiplayer;
using fluXis.Game.Screens.Ranking;
using fluXis.Game.Screens.Select;
using fluXis.Game.UI;
using fluXis.Game.Utils.Extensions;
using fluXis.Shared.Components.Users;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Bindables;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Input;
using osu.Framework.Input.Events;
using osu.Framework.Platform;
using osu.Framework.Screens;
using osuTK;
using osuTK.Graphics;
using osuTK.Input;

namespace fluXis.Game.Screens.Menu;

public partial class MenuScreen : FluXisScreen
{
    public override float Zoom => pressedStart ? 1f : 1.2f;
    public override bool ShowToolbar => pressedStart;
    public override bool AutoPlayNext => true;

    [Resolved]
    private MapStore maps { get; set; }

    [Resolved]
    private GlobalBackground backgrounds { get; set; }

    [Resolved]
    private SettingsMenu settings { get; set; }

    [Resolved]
    private LoginOverlay login { get; set; }

    [Resolved]
    private GlobalClock clock { get; set; }

    [Resolved]
    private IAPIClient api { get; set; }

    [Resolved]
    private FluXisConfig config { get; set; }

    [Resolved]
    private Toolbar toolbar { get; set; }

    [Resolved]
    private Dashboard dashboard { get; set; }

    [Resolved]
    private PanelContainer panels { get; set; }

    private FluXisTextFlow splashText;
    private FluXisSpriteText pressAnyKeyText;

    private ParallaxContainer visualizerContainer;
    private MenuVisualizer visualizer;
    private ParallaxContainer snowContainer;

    private bool shouldSnow => Game.CurrentSeason == Season.Winter || forceSnow.Value;

    private Bindable<bool> forceSnow;

    private Container textContainer;
    private Container buttons;
    private FillFlowContainer linkContainer;
    private MenuUpdates updates;

    private Sprite logoText;
    private CircularContainer animationCircle;

    private MenuImageButton playButton;
    private double mapChangeTime;
    private int mapCount;

    private MenuImageButton multiButton;
    private MenuLongButton dashboardButton;
    private MenuLongButton browseButton;

    private bool pressedStart;
    private double inactivityTime;
    private const double inactivity_timeout = 60 * 1000;

    [BackgroundDependencyLoader]
    private void load(GameHost host, TextureStore textures)
    {
        forceSnow = config.GetBindable<bool>(FluXisSetting.ForceSnow);

        InternalChildren = new Drawable[]
        {
            visualizerContainer = new ParallaxContainer
            {
                Child = visualizer = new MenuVisualizer(),
                RelativeSizeAxes = Axes.Both,
                Strength = .1f,
                Alpha = shouldSnow ? 0 : 1
            },
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Padding = new MarginPadding(40),
                Children = new Drawable[]
                {
                    new GridContainer
                    {
                        RelativeSizeAxes = Axes.Both,
                        ColumnDimensions = new[] { new Dimension(), },
                        RowDimensions = new Dimension[]
                        {
                            new(), // title/splash
                            new(GridSizeMode.AutoSize), // buttons
                            new() // empty
                        },
                        Content = new[]
                        {
                            new Drawable[]
                            {
                                textContainer = new Container
                                {
                                    RelativeSizeAxes = Axes.X,
                                    AutoSizeAxes = Axes.Y,
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,
                                    Alpha = 0,
                                    Y = -50,
                                    Children = new Drawable[]
                                    {
                                        logoText = new Sprite
                                        {
                                            Texture = textures.Get("Logos/logo-text-shadow"),
                                            Scale = new Vector2(.5f),
                                            Anchor = Anchor.TopCentre,
                                            Origin = Anchor.TopCentre
                                        },
                                        splashText = new FluXisTextFlow
                                        {
                                            Width = 1280,
                                            AutoSizeAxes = Axes.Y,
                                            Anchor = Anchor.TopCentre,
                                            Origin = Anchor.TopCentre,
                                            FontSize = FluXisSpriteText.GetWebFontSize(24),
                                            TextAnchor = Anchor.TopCentre,
                                            Margin = new MarginPadding { Top = 88 },
                                            Shadow = true
                                        }
                                    }
                                }
                            },
                            new Drawable[]
                            {
                                buttons = new Container
                                {
                                    AutoSizeAxes = Axes.Both,
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,
                                    Shear = new Vector2(MenuButtonBase.SHEAR_AMOUNT, 0),
                                    Children = new Drawable[]
                                    {
                                        playButton = new MenuImageButton
                                        {
                                            Text = LocalizationStrings.MainMenu.PlayText,
                                            Icon = FontAwesome6.Solid.Play,
                                            Keys = new[] { Key.Enter, Key.P },
                                            GamepadButton = JoystickButton.Button2, // A
                                            Action = continueToPlay,
                                            Size = new Vector2(350, 200)
                                        },
                                        multiButton = new MenuImageButton
                                        {
                                            Text = LocalizationStrings.MainMenu.MultiplayerText,
                                            Description = LocalizationStrings.MainMenu.MultiplayerDescription,
                                            Icon = FontAwesome6.Solid.Users,
                                            Keys = new[] { Key.M },
                                            GamepadButton = JoystickButton.Button3, // B
                                            Action = continueToMultiplayer,
                                            DefaultSprite = new Sprite
                                            {
                                                Size = new Vector2(1),
                                                Texture = textures.Get("Multiplayer/button-lobby")
                                            },
                                            Size = new Vector2(260, 200),
                                            Position = new Vector2(370, 0),
                                            Column = 2
                                        },
                                        new MenuImageButton
                                        {
                                            Text = LocalizationStrings.MainMenu.EditText,
                                            Description = LocalizationStrings.MainMenu.EditDescription,
                                            Icon = FontAwesome6.Solid.PenRuler,
                                            Keys = new[] { Key.E },
                                            Action = () => this.Push(new EditorLoader()),
                                            DefaultSprite = new Sprite
                                            {
                                                Size = new Vector2(1),
                                                Texture = textures.Get("menu-edit")
                                            },
                                            Size = new Vector2(260, 200),
                                            Position = new Vector2(650, 0),
                                            Column = 3
                                        },
                                        dashboardButton = new MenuLongButton
                                        {
                                            Text = LocalizationStrings.MainMenu.DashboardText,
                                            Description = LocalizationStrings.MainMenu.DashboardDescription,
                                            Icon = FontAwesome6.Solid.ChartLine,
                                            Keys = new[] { Key.D },
                                            GamepadButton = JoystickButton.Button1, // X
                                            Action = openDashboard,
                                            Size = new Vector2(385, 80),
                                            Position = new Vector2(0, 220),
                                            Row = 2
                                        },
                                        browseButton = new MenuLongButton
                                        {
                                            Text = LocalizationStrings.MainMenu.BrowseText,
                                            Description = LocalizationStrings.MainMenu.BrowseDescription,
                                            Icon = FontAwesome6.Solid.EarthAmericas,
                                            GamepadButton = JoystickButton.Button4, // Y
                                            Keys = new[] { Key.B },
                                            Action = continueToBrowse,
                                            Size = new Vector2(385, 80),
                                            Position = new Vector2(405, 220),
                                            Row = 2,
                                            Column = 2
                                        },
                                        new MenuSmallButton
                                        {
                                            Icon = FontAwesome6.Solid.DoorOpen,
                                            Action = Game.Exit,
                                            GamepadButton = JoystickButton.Button9, // Back
                                            Size = new Vector2(100, 80),
                                            Position = new Vector2(810, 220),
                                            Row = 2,
                                            Column = 3
                                        }
                                    }
                                }
                            },
                            new[] { Empty() }
                        }
                    },
                    new Container
                    {
                        RelativeSizeAxes = Axes.Both,
                        Children = new Drawable[]
                        {
                            animationCircle = new CircularContainer
                            {
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                Masking = true,
                                BorderColour = Color4.White,
                                BorderThickness = 60,
                                Children = new Drawable[]
                                {
                                    new Box
                                    {
                                        RelativeSizeAxes = Axes.Both,
                                        AlwaysPresent = true,
                                        Alpha = 0
                                    }
                                }
                            },
                            logoText = new Sprite
                            {
                                Texture = textures.Get("Logos/logo-text-shadow"),
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre
                            }
                        }
                    },
                    pressAnyKeyText = new FluXisSpriteText
                    {
                        Text = LocalizationStrings.MainMenu.PressAnyKey,
                        FontSize = 32,
                        Shadow = true,
                        Anchor = Anchor.BottomCentre,
                        Origin = Anchor.BottomCentre
                    },
                    new MenuNowPlaying(),
                    updates = new MenuUpdates { X = 200 },
                    linkContainer = new FillFlowContainer
                    {
                        AutoSizeAxes = Axes.Both,
                        Anchor = Anchor.BottomLeft,
                        Origin = Anchor.BottomLeft,
                        Direction = FillDirection.Horizontal,
                        Spacing = new Vector2(10),
                        Alpha = 0,
                        X = -200,
                        Children = new Drawable[]
                        {
                            new MenuLinkButton
                            {
                                Icon = FontAwesome6.Brands.Discord,
                                Action = () => host.OpenUrlExternally("https://discord.gg/29hMftpNq9"),
                                Text = "Discord"
                            },
                            new MenuLinkButton
                            {
                                Icon = FontAwesome6.Brands.GitHub,
                                Action = () => host.OpenUrlExternally("https://github.com/InventiveRhythm/fluXis"),
                                Text = "GitHub"
                            },
                            new MenuLinkButton
                            {
                                Icon = FontAwesome6.Solid.EarthAmericas,
                                Action = () => host.OpenUrlExternally(api.Endpoint.WebsiteRootUrl),
                                Text = "Website"
                            }
                        }
                    }
                }
            },
            snowContainer = new ParallaxContainer
            {
                RelativeSizeAxes = Axes.Both,
                Child = new MenuSnow(),
                Strength = .05f,
                Alpha = shouldSnow ? 1 : 0
            }
        };

        mapCount = maps.MapSets.Count;
        maps.CollectionUpdated += () => Schedule(() => this.TransformTo(nameof(mapCount), maps.MapSets.Count, 500, Easing.OutQuint));
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        forceSnow.BindValueChanged(_ =>
        {
            visualizerContainer.FadeTo(shouldSnow ? 0 : 1, 600, Easing.OutQuint);
            snowContainer.FadeTo(shouldSnow ? 1 : 0, 600, Easing.OutQuint);
        });

        api.User.BindValueChanged(updateButtons, true);
    }

    private void updateButtons(ValueChangedEvent<APIUser> e)
    {
        Scheduler.ScheduleIfNeeded(() =>
        {
            var enabled = e.NewValue != null;
            multiButton.Enabled.Value = enabled;
            dashboardButton.Enabled.Value = enabled;
            browseButton.Enabled.Value = enabled;
        });
    }

    private void continueToPlay() => this.Push(new SelectScreen());
    private void continueToMultiplayer() => this.Push(new MultiplayerScreen());
    private void continueToRankings() => this.Push(new Rankings());
    private void openDashboard() => dashboard.Show();
    private void continueToBrowse() => this.Push(new MapBrowser());

    public bool CanPlayAnimation()
    {
        if (pressedStart) return false;

        playStartAnimation();
        return true;
    }

    private void playStartAnimation()
    {
        pressedStart = true;
        inactivityTime = 0;
        UISamples?.Select();
        randomizeSplash();
        backgrounds.Zoom = 1f;

        logoText.ScaleTo(1.1f, 1000, Easing.OutQuint).FadeOut(600);
        animationCircle.BorderTo(60f).ResizeTo(0)
                       .BorderTo(0f, 1200, Easing.OutQuint)
                       .ResizeTo(400, 1000, Easing.OutQuint);

        this.Delay(800).FadeIn().OnComplete(_ =>
        {
            toolbar.Show();
            showMenu(true);
            login.Show();
        });

        pressAnyKeyText.FadeOut(600).MoveToY(200, 800, Easing.InQuint);
    }

    private void revertStartAnimation()
    {
        toolbar.Hide();
        backgrounds.Zoom = 1.2f;
        hideMenu();

        logoText.Delay(800).ScaleTo(.9f).ScaleTo(1f, 800, Easing.OutQuint).FadeIn(400);
        this.Delay(800).FadeIn().OnComplete(_ => pressedStart = false);

        pressAnyKeyText.Delay(800).MoveToY(0, 800, Easing.OutQuint);
        pressAnyKeyText.FadeInFromZero(1400).Then().FadeOut(1400).Loop();
    }

    protected override bool OnKeyDown(KeyDownEvent e)
    {
        if (e.Key == Key.Escape)
        {
            panels.Content ??= new ConfirmExitPanel();
            return true;
        }

        return CanPlayAnimation();
    }

    protected override bool OnMouseDown(MouseDownEvent e) => CanPlayAnimation();
    protected override bool OnTouchDown(TouchDownEvent e) => CanPlayAnimation();
    protected override bool OnMidiDown(MidiDownEvent e) => CanPlayAnimation();

    protected override bool OnJoystickPress(JoystickPressEvent e)
    {
        if (CanPlayAnimation()) return true;

        switch (e.Button)
        {
            case JoystickButton.Button10: // Start
                settings.ToggleVisibility();
                return true;
        }

        return false;
    }

    private void showMenu(bool longer = false)
    {
        // we don't need the delay
        var delay = longer ? 0 : ENTER_DELAY;

        using (BeginDelayedSequence(delay))
        {
            var moveDuration = longer ? 1000 : MOVE_DURATION;
            var fadeDuration = longer ? 800 : FADE_DURATION;

            textContainer.MoveToY(0, moveDuration, Easing.OutQuint).FadeIn(fadeDuration);
            buttons.ForEach(b => b.Show());
            linkContainer.MoveToX(0, moveDuration, Easing.OutQuint).FadeIn(fadeDuration);

            updates.CanShow = true;
            updates.Show(moveDuration, fadeDuration);
        }
    }

    private void hideMenu()
    {
        textContainer.MoveToY(-50, MOVE_DURATION, Easing.OutQuint).FadeOut(FADE_DURATION);
        buttons.ForEach(b => b.Hide());
        linkContainer.MoveToX(-200, MOVE_DURATION, Easing.OutQuint).FadeOut(FADE_DURATION);

        updates.CanShow = false;
        updates.Hide();
    }

    private void randomizeSplash() => splashText.Text = MenuSplashes.RandomSplash;

    public void PreEnter()
    {
        if (config.Get<bool>(FluXisSetting.IntroTheme))
        {
            maps.CurrentMap = maps.CreateBuiltinMap(Game.CurrentSeason == Season.Halloween ? MapStore.BuiltinMap.Spoophouse : MapStore.BuiltinMap.Roundhouse).LowestDifficulty;
            clock.Seek(0);
        }
        else // if disabled, load a random map
            maps.CurrentMap = maps.GetRandom()?.Maps.FirstOrDefault() ?? MapStore.CreateDummyMap();

        clock.Stop();
        clock.VolumeTo(0);

        backgrounds.AddBackgroundFromMap(maps.CurrentMapSet?.Maps.First());
    }

    public override void OnEntering(ScreenTransitionEvent e)
    {
        clock.VolumeTo(0).VolumeTo(1, 500);
        clock.Start();

        if (config.Get<bool>(FluXisSetting.IntroTheme))
            clock.Seek(0);
        else
            clock.Seek(maps.CurrentMapSet?.Metadata?.PreviewTime ?? 0);

        pressAnyKeyText.FadeInFromZero(1400).Then().FadeOut(1400).Loop();
        inactivityTime = 0;

        if (!shouldSnow)
            visualizer.FadeInFromZero(2000);
    }

    public override void OnSuspending(ScreenTransitionEvent e)
    {
        this.FadeOut(300);
        hideMenu();
    }

    public override void OnResuming(ScreenTransitionEvent e)
    {
        using (BeginDelayedSequence(ENTER_DELAY))
            showMenu();

        randomizeSplash();
        this.FadeIn(300);
        inactivityTime = 0;
    }

    protected override bool OnMouseMove(MouseMoveEvent e)
    {
        inactivityTime = 0;
        return false;
    }

    protected override void Update()
    {
        playButton.Description = LocalizationStrings.MainMenu.PlayDescription(mapCount);

        if (!Game.AnyOverlayOpen)
            inactivityTime += Time.Elapsed;

        if (inactivityTime > inactivity_timeout && pressedStart)
        {
            inactivityTime = 0;
            revertStartAnimation();
        }

        if (playButton.IsVisible)
            mapChangeTime -= Time.Elapsed;

        if (mapChangeTime <= 0)
        {
            mapChangeTime = 5000;

            var map = maps.GetRandom()?.Maps.FirstOrDefault() ?? MapStore.CreateDummyMap();

            LoadComponentAsync(new BlurableBackground(map, .2f), b =>
            {
                playButton.Stack.AutoFill = false;
                playButton.Stack.Add(b);
            });
        }
    }
}
