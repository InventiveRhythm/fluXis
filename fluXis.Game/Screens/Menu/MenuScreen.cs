using System.Linq;
using fluXis.Game.Activity;
using fluXis.Game.Audio;
using fluXis.Game.Database.Maps;
using fluXis.Game.Graphics;
using fluXis.Game.Graphics.Background;
using fluXis.Game.Graphics.Panel;
using fluXis.Game.Map;
using fluXis.Game.Online.Fluxel;
using fluXis.Game.Overlay.Login;
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
using osuTK.Input;

namespace fluXis.Game.Screens.Menu;

public partial class MenuScreen : FluXisScreen
{
    public override float Zoom => pressedStart ? 1f : 1.2f;
    public override float BackgroundDim => pressedStart ? base.BackgroundDim : 1f;
    public override bool ShowToolbar => pressedStart;

    [Resolved]
    private MapStore maps { get; set; }

    [Resolved]
    private BackgroundStack backgrounds { get; set; }

    [Resolved]
    private SettingsMenu settings { get; set; }

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
    private double inactivityTime;
    private const double inactivity_timeout = 60 * 1000;

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
                                X = -200
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
                                Action = continueToPlay,
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
                                Action = continueToMultiplayer,
                                Width = 300,
                                X = 110,
                                Y = 80
                            },
                            new MenuButton
                            {
                                Text = "Ranking",
                                Description = "Check online leaderboards",
                                Icon = FontAwesome.Solid.Trophy,
                                Action = continueToRankings,
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
                                Action = continueToBrowse
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
                    new MenuGamepadTooltips
                    {
                        ButtonContainer = buttonContainer
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
    }

    private void continueToPlay() => this.Push(new SelectScreen());
    private void continueToMultiplayer() => this.Push(new MultiplayerScreen());
    private void continueToRankings() => this.Push(new Rankings());
    private void continueToBrowse() => this.Push(new Bluescreen());

    private bool canPlayAnimation()
    {
        if (pressedStart) return false;

        playStartAnimation();
        return true;
    }

    private void playStartAnimation()
    {
        pressedStart = true;
        inactivityTime = 0;
        menuStart?.Play();
        randomizeSplash();
        backgrounds.Zoom = 1f;

        fluXisText.MoveTo(Vector2.Zero, 1000, Easing.InOutCirc);
        fluXisText.Delay(800).FadeIn().OnComplete(_ =>
        {
            game.Toolbar.ShowToolbar.Value = true;
            splashText.MoveToX(0, 600, Easing.OutQuint).FadeIn(300);
            showMenu(600);
            login.Show();
        });

        pressAnyKeyText.FadeOut(600).MoveToY(200, 800, Easing.InQuint);
    }

    private void revertStartAnimation()
    {
        game.Toolbar.ShowToolbar.Value = false;
        backgrounds.Zoom = 1.2f;
        splashText.MoveToX(-200, 600, Easing.InQuint).FadeOut(300);
        buttonContainer.MoveToX(-200, 600, Easing.InQuint).FadeOut(300);
        linkContainer.MoveToX(200, 600, Easing.InQuint).FadeOut(300);

        var vec = new Vector2(content.DrawWidth / 2 - fluXisText.DrawWidth / 2 - 40, (content.DrawHeight + 50) / 2 - fluXisText.DrawHeight / 2 - 40);

        fluXisText.MoveTo(vec, 1000, Easing.InOutCirc)
                  .Delay(800).FadeIn().OnComplete(_ => pressedStart = false);

        pressAnyKeyText.MoveToY(0, 800, Easing.OutQuint);
        pressAnyKeyText.FadeInFromZero(1400).Then().FadeOut(1400).Loop();
    }

    protected override bool OnKeyDown(KeyDownEvent e)
    {
        if (e.Key == Key.Escape)
        {
            game.Overlay ??= new ConfirmExitPanel();
            return true;
        }

        return canPlayAnimation();
    }

    protected override bool OnTouchDown(TouchDownEvent e) => canPlayAnimation();
    protected override bool OnMidiDown(MidiDownEvent e) => canPlayAnimation();

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

        pressAnyKeyText.FadeInFromZero(1400).Then().FadeOut(1400).Loop();
        fluXisText.FadeInFromZero(800);
        visualizer.FadeInFromZero(2000);
        backgrounds.SetDim(1f, 0);
        backgrounds.SetDim(base.BackgroundDim, 2000);
        inactivityTime = 0;
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
        inactivityTime = 0;
    }

    protected override void Update()
    {
        if (clock.Finished) game.NextSong();

        if (!pressedStart)
        {
            fluXisText.X = content.DrawWidth / 2 - fluXisText.DrawWidth / 2 - 40;
            fluXisText.Y = content.DrawHeight / 2 - fluXisText.DrawHeight / 2 - 40;
        }

        inactivityTime += Time.Elapsed;

        if (inactivityTime > inactivity_timeout && pressedStart)
        {
            inactivityTime = 0;
            revertStartAnimation();
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
