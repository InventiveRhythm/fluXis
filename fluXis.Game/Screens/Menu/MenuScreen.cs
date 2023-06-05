using System.Linq;
using fluXis.Game.Audio;
using fluXis.Game.Database.Maps;
using fluXis.Game.Graphics;
using fluXis.Game.Graphics.Background;
using fluXis.Game.Map;
using fluXis.Game.Overlay.Login;
using fluXis.Game.Overlay.Mouse;
using fluXis.Game.Overlay.Notification;
using fluXis.Game.Overlay.Settings;
using fluXis.Game.Screens.Browse;
using fluXis.Game.Screens.Edit;
using fluXis.Game.Screens.Menu.UI;
using fluXis.Game.Screens.Menu.UI.Visualizer;
using fluXis.Game.Screens.Select;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Platform;
using osu.Framework.Screens;
using osuTK;

namespace fluXis.Game.Screens.Menu;

public partial class MenuScreen : FluXisScreen
{
    public override float Zoom => 1f;

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

    private Box blackBox;
    private Container content;

    private Container textContainer;
    private Container buttonContainer;
    private FillFlowContainer linkContainer;

    private MenuButton playButton;

    private FluXisSpriteText titleText;
    private FluXisSpriteText artistText;

    [BackgroundDependencyLoader]
    private void load(GameHost host)
    {
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
                Child = new MenuVisualizer(),
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
                        Children = new[]
                        {
                            new FluXisSpriteText
                            {
                                Text = "fluXis",
                                FontSize = 100,
                                Shadow = true,
                                ShadowOffset = new Vector2(0, 0.04f)
                            },
                            new FluXisSpriteText
                            {
                                Text = "A free and community-driven rhythm game",
                                FontSize = 32,
                                Margin = new MarginPadding { Top = 80 },
                                Shadow = true
                            },
                        }
                    },
                    new FillFlowContainer()
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
                        Children = new Drawable[]
                        {
                            playButton = new MenuButton
                            {
                                Text = "Play!",
                                Description = $"{maps.MapSets.Count} maps loaded",
                                Icon = FontAwesome.Solid.Play,
                                Action = () => this.Push(new SelectScreen()),
                                Width = 700
                            },
                            new MenuButton
                            {
                                Text = "Browse",
                                Description = "Download community-made maps",
                                Icon = FontAwesome.Solid.Download,
                                Width = 340,
                                Margin = new MarginPadding { Top = 80 },
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
                                Action = () => this.Push(new Editor()),
                                Width = 340,
                                Margin = new MarginPadding { Top = 80, Left = 360 }
                            },
                            new SmallMenuButton
                            {
                                Text = "Settings",
                                Icon = FontAwesome.Solid.Cog,
                                Action = settings.ToggleVisibility,
                                Width = 340,
                                Margin = new MarginPadding { Top = 160 }
                            },
                            new SmallMenuButton
                            {
                                Text = "Exit",
                                Icon = FontAwesome.Solid.Times,
                                Action = game.Exit,
                                Width = 340,
                                Margin = new MarginPadding { Top = 160, Left = 360 }
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
                                Action = () => host.OpenUrlExternally("https://fluxis.foxes4life.net"),
                                Text = "Website"
                            }
                        }
                    }
                }
            },
            blackBox = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Colour4.Black,
                Alpha = 0
            }
        };

        Schedule(() => login.Show());

        maps.MapSetAdded += _ => playButton.Description = $"{maps.MapSets.Count} maps loaded";
    }

    protected override void LoadComplete()
    {
        game.OnSongChanged += songChanged;
    }

    public override void OnEntering(ScreenTransitionEvent e)
    {
        this.FadeInFromZero(200);

        textContainer.MoveToX(-200).MoveToX(0, 400, Easing.OutQuint);
        buttonContainer.MoveToX(-200).MoveToX(0, 400, Easing.OutQuint);
        linkContainer.MoveToX(200).MoveToX(0, 400, Easing.OutQuint);
        // profile.MoveToX(200).MoveToX(0, 400, Easing.OutQuint);

        base.OnEntering(e);
    }

    public override void OnSuspending(ScreenTransitionEvent e)
    {
        this.FadeOut(200);

        textContainer.MoveToX(-200, 400, Easing.OutQuint);
        buttonContainer.MoveToX(-200, 400, Easing.OutQuint);
        linkContainer.MoveToX(200, 400, Easing.OutQuint);
        // profile.MoveToX(200, 400, Easing.OutQuint);

        base.OnSuspending(e);
    }

    public override void OnResuming(ScreenTransitionEvent e)
    {
        this.FadeIn(200);

        textContainer.MoveToX(0, 400, Easing.OutQuint);
        buttonContainer.MoveToX(0, 400, Easing.OutQuint);
        linkContainer.MoveToX(0, 400, Easing.OutQuint);
        // profile.MoveToX(0, 400, Easing.OutQuint);

        base.OnResuming(e);
    }

    protected override void Update()
    {
        if (clock.Finished) game.NextSong();

        base.Update();
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
