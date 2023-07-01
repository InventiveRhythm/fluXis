using System.Linq;
using fluXis.Game.Activity;
using fluXis.Game.Audio;
using fluXis.Game.Database.Maps;
using fluXis.Game.Graphics;
using fluXis.Game.Graphics.Background;
using fluXis.Game.Map;
using fluXis.Game.Online.Fluxel;
using fluXis.Game.Overlay.Login;
using fluXis.Game.Overlay.Mouse;
using fluXis.Game.Overlay.Notification;
using fluXis.Game.Overlay.Settings;
using fluXis.Game.Screens.Browse;
using fluXis.Game.Screens.Edit;
using fluXis.Game.Screens.Menu.UI;
using fluXis.Game.Screens.Menu.UI.Visualizer;
using fluXis.Game.Screens.Multiplayer;
using fluXis.Game.Screens.Ranking;
using fluXis.Game.Screens.Select;
using osu.Framework.Allocation;
using osu.Framework.Audio.Sample;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osu.Framework.Platform;
using osu.Framework.Screens;
using osuTK;

namespace fluXis.Game.Screens.Menu;

public partial class MenuScreen : FluXisScreen
{
    public override float Zoom => 1f;
    public override float BackgroundDim => pressedStart ? default : 1f;
    public override bool ShowToolbar => pressedStart;

    [Resolved]
    private MapStore maps { get; set; }

    [Resolved]
    private BackgroundStack backgrounds { get; set; }

    [Resolved]
    private SettingsMenu settings { get; set; }

    [Resolved]
    private GlobalCursorOverlay cursorOverlay { get; set; }

    [Resolved]
    private NotificationOverlay notifications { get; set; }

    [Resolved]
    private LoginOverlay login { get; set; }

    [Resolved]
    private FluXisGameBase game { get; set; }

    [Resolved]
    private AudioClock clock { get; set; }

    [Resolved]
    private Fluxel fluxel { get; set; }

    [Resolved]
    private ActivityManager activity { get; set; }

    private Container content;
    private FluXisSpriteText fluXisText;
    private FluXisTextFlow splashText;
    private FluXisSpriteText pressAnyKeyText;
    private MenuVisualizer visualizer;

    private Container textContainer;
    private Container buttonContainer;
    private FillFlowContainer linkContainer;

    private MenuPlayButton playButton;

    private FluXisSpriteText titleText;
    private FluXisSpriteText artistText;

    private bool pressedStart;
    private Sample menuStart;

    [BackgroundDependencyLoader]
    private void load(GameHost host, ISampleStore samples)
    {
        menuStart = samples.Get("UI/accept");

        // load a random map
        if (maps.MapSets.Count > 0)
        {
            maps.CurrentMapSet = maps.GetRandom();

            RealmMap map = maps.CurrentMapSet.Maps.First();
            clock.LoadMap(map, true, true);
            Schedule(songChanged);
        }

        backgrounds.AddBackgroundFromMap(maps.CurrentMapSet?.Maps.First());

        InternalChildren = new Drawable[]
        {
            new ParallaxContainer
            {
                Child = visualizer = new MenuVisualizer(),
                RelativeSizeAxes = Axes.Both,
                Strength = 5
            },
            content = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Padding = new MarginPadding(40),
                Children = new Drawable[]
                {
                    textContainer = new Container
                    {
                        RelativeSizeAxes = Axes.Both,
                        Children = new Drawable[]
                        {
                            fluXisText = new FluXisSpriteText
                            {
                                Text = "fluXis",
                                FontSize = 100,
                                Shadow = true,
                                ShadowOffset = new Vector2(0, 0.04f)
                            },
                            splashText = new FluXisTextFlow
                            {
                                FontSize = 32,
                                RelativeSizeAxes = Axes.X,
                                Margin = new MarginPadding { Top = 80 },
                                Shadow = true,
                                Alpha = 0,
                                X = -200,
                            },
                            pressAnyKeyText = new FluXisSpriteText
                            {
                                Text = "Press any key.",
                                FontSize = 32,
                                Shadow = true,
                                Anchor = Anchor.BottomCentre,
                                Origin = Anchor.BottomCentre
                            }
                        }
                    },
                    new FillFlowContainer
                    {
                        AutoSizeAxes = Axes.Both,
                        Direction = FillDirection.Vertical,
                        Anchor = Anchor.TopRight,
                        Origin = Anchor.TopRight,
                        Children = new Drawable[]
                        {
                            titleText = new FluXisSpriteText
                            {
                                FontSize = 32,
                                Shadow = true,
                                Anchor = Anchor.TopRight,
                                Origin = Anchor.TopRight
                            },
                            artistText = new FluXisSpriteText
                            {
                                FontSize = 22,
                                Colour = FluXisColors.Text2,
                                Shadow = true,
                                Anchor = Anchor.TopRight,
                                Origin = Anchor.TopRight
                            }
                        }
                    },
                    buttonContainer = new Container
                    {
                        AutoSizeAxes = Axes.Both,
                        Anchor = Anchor.BottomLeft,
                        Origin = Anchor.BottomLeft,
                        Alpha = 0,
                        X = -200,
                        Children = new Drawable[]
                        {
                            playButton = new MenuPlayButton
                            {
                                Description = $"{maps.MapSets.Count} maps loaded",
                                Action = () => this.Push(new SelectScreen()),
                                Width = 710
                            },
                            new SmallMenuButton
                            {
                                Icon = FontAwesome.Solid.Cog,
                                Action = settings.ToggleVisibility,
                                Width = 100,
                                Y = 80
                            },
                            new MenuButton
                            {
                                Text = "Multiplayer",
                                Description = "Play against other players",
                                Icon = FontAwesome.Solid.Users,
                                Action = () => this.Push(new MultiplayerScreen()),
                                Width = 300,
                                X = 110,
                                Y = 80
                            },
                            new MenuButton
                            {
                                Text = "Ranking",
                                Description = "Check online leaderboards",
                                Icon = FontAwesome.Solid.Trophy,
                                Action = () => this.Push(new Rankings()),
                                Width = 290,
                                X = 430,
                                Y = 80
                            },
                            new SmallMenuButton
                            {
                                Icon = FontAwesome.Solid.Times,
                                Action = game.Exit,
                                Width = 120,
                                Y = 160
                            },
                            new MenuButton
                            {
                                Text = "Browse",
                                Description = "Download community-made maps",
                                Icon = FontAwesome.Solid.Download,
                                Width = 340,
                                X = 130,
                                Y = 160,
                                Action = () =>
                                {
                                    clock.Stop();
                                    this.Push(new Bluescreen());
                                }
                            },
                            new MenuButton
                            {
                                Text = "Edit",
                                Description = "Create your own maps",
                                Icon = FontAwesome.Solid.Pen,
                                ShearAdjust = .05f,
                                Action = () => this.Push(new Editor()),
                                Width = 250,
                                X = 490,
                                Y = 160
                            }
                        }
                    },
                    linkContainer = new FillFlowContainer
                    {
                        AutoSizeAxes = Axes.Both,
                        Anchor = Anchor.BottomRight,
                        Origin = Anchor.BottomRight,
                        Direction = FillDirection.Horizontal,
                        Spacing = new Vector2(10),
                        Alpha = 0,
                        X = 200,
                        Children = new Drawable[]
                        {
                            new MenuIconButton
                            {
                                Icon = FontAwesome.Brands.Discord,
                                Action = () => host.OpenUrlExternally("https://discord.gg/29hMftpNq9"),
                                Text = "Discord"
                            },
                            new MenuIconButton
                            {
                                Icon = FontAwesome.Brands.Github,
                                Action = () => host.OpenUrlExternally("https://github.com/TeamFluXis/fluXis"),
                                Text = "GitHub"
                            },
                            new MenuIconButton
                            {
                                Icon = FontAwesome.Solid.Globe,
                                Action = () => host.OpenUrlExternally(fluxel.Endpoint.WebsiteRootUrl),
                                Text = "Website"
                            }
                        }
                    }
                }
            }
        };

        maps.MapSetAdded += _ => playButton.Description = $"{maps.MapSets.Count} maps loaded";
    }

    protected override void LoadComplete()
    {
        game.OnSongChanged += songChanged;

        pressAnyKeyText.FadeInFromZero(800).Then().FadeOut(800).Loop();
        backgrounds.SetDim(base.BackgroundDim, 800);
        visualizer.FadeInFromZero(800);
    }

    protected override bool OnKeyDown(KeyDownEvent e)
    {
        if (!pressedStart)
        {
            pressedStart = true;
            menuStart?.Play();
            randomizeSplash();

            fluXisText.MoveTo(Vector2.Zero, 1000, Easing.InOutCirc);
            fluXisText.Delay(600).FadeIn().OnComplete(_ =>
            {
                game.Toolbar.ShowToolbar.Value = true;
                splashText.MoveToX(0, 600, Easing.OutQuint).FadeIn(300);
                showMenu(600);
                login.Show();
            });

            pressAnyKeyText.FadeOut(600).MoveToY(0, 800, Easing.InQuint);

            return true;
        }

        return false;
    }

    private void showMenu(int duration = 400)
    {
        textContainer.MoveToX(0, duration, Easing.OutQuint).FadeIn(duration / 2f);
        buttonContainer.MoveToX(0, duration, Easing.OutQuint).FadeIn(duration / 2f);
        linkContainer.MoveToX(0, duration, Easing.OutQuint).FadeIn(duration / 2f);
    }

    private void hideMenu(int duration = 400)
    {
        textContainer.MoveToX(-200, duration, Easing.OutQuint);
        buttonContainer.MoveToX(-200, duration, Easing.OutQuint);
        linkContainer.MoveToX(200, duration, Easing.OutQuint);
    }

    private void randomizeSplash() => splashText.Text = MenuSplashes.RandomSplash;

    public override void OnEntering(ScreenTransitionEvent e)
    {
        activity.Update("In the menus", "Idle", "menu");
    }

    public override void OnSuspending(ScreenTransitionEvent e)
    {
        this.FadeOut(200);
        hideMenu();
    }

    public override void OnResuming(ScreenTransitionEvent e)
    {
        randomizeSplash();
        showMenu();
        this.FadeIn(200);
        activity.Update("In the menus", "Idle", "menu");
    }

    protected override void Update()
    {
        if (clock.Finished) game.NextSong();

        if (!pressedStart)
        {
            fluXisText.X = content.DrawWidth / 2 - fluXisText.DrawWidth / 2 - 40;
            fluXisText.Y = content.DrawHeight / 2 - fluXisText.DrawHeight / 2 - 40;
        }
    }

    private void songChanged()
    {
        titleText.Text = maps.CurrentMapSet.Metadata.Title;
        artistText.Text = maps.CurrentMapSet.Metadata.Artist;

        titleText.FadeInFromZero(400).MoveToX(200).MoveToX(0, 800, Easing.OutCirc)
                 .Then(2000).FadeOut(400);

        artistText.FadeInFromZero(400).MoveToX(200).MoveToX(0, 800, Easing.OutCirc)
                  .Then(2000).FadeOut(400);
    }
}
