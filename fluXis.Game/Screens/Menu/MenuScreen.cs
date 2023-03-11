using System.Linq;
using fluXis.Game.Audio;
using fluXis.Game.Database.Maps;
using fluXis.Game.Graphics.Background;
using fluXis.Game.Map;
using fluXis.Game.Overlay.Mouse;
using fluXis.Game.Overlay.Notification;
using fluXis.Game.Overlay.Settings;
using fluXis.Game.Screens.Edit;
using fluXis.Game.Screens.Menu.UI;
using fluXis.Game.Screens.Menu.UI.Profile;
using fluXis.Game.Screens.Menu.UI.Visualizer;
using fluXis.Game.Screens.Select;
using fluXis.Game.Utils;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Platform;
using osu.Framework.Screens;
using osuTK;

namespace fluXis.Game.Screens.Menu;

public partial class MenuScreen : Screen
{
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

    private Box blackBox;
    private Container content;

    private MenuButton playButton;

    [BackgroundDependencyLoader]
    private void load(Storage storage, GameHost host)
    {
        // load a random map
        if (maps.MapSets.Count > 0)
        {
            maps.CurrentMapSet = maps.GetRandom();

            RealmMap map = maps.CurrentMapSet.Maps.First();
            Conductor.PlayTrack(map, true, map.Metadata.PreviewTime);
        }

        backgrounds.AddBackgroundFromMap(maps.CurrentMapSet?.Maps.First());

        InternalChildren = new Drawable[]
        {
            new MenuVisualizer(),
            content = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Padding = new MarginPadding(40),
                Children = new Drawable[]
                {
                    new SpriteText
                    {
                        Text = "fluXis",
                        Font = new FontUsage("Quicksand", 100, "Bold"),
                        Y = -20
                    },
                    new SpriteText
                    {
                        Text = "A free and community-driven rhythm game",
                        Font = new FontUsage("Quicksand", 32),
                        Margin = new MarginPadding { Top = 60 }
                    },
                    new Container
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
                                Action = () => notifications.AddNotification(new Notification("Coming soon!", "This feature is not yet implemented."))
                            },
                            new MenuButton
                            {
                                Text = "Edit",
                                Description = "Create your own maps",
                                Icon = FontAwesome.Solid.Pen,
                                Action = () =>
                                {
                                    RealmMap map = maps.CurrentMapSet.Maps.First();
                                    MapInfo mapInfo = MapUtils.LoadFromPath(storage.GetFullPath("files/" + PathUtils.HashToPath(map.Hash)));
                                    this.Push(new Editor(map, mapInfo));
                                },
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
                                Action = exit,
                                Width = 340,
                                Margin = new MarginPadding { Top = 160, Left = 360 }
                            }
                        }
                    },
                    new FillFlowContainer
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
                    },
                    new MenuProfile()
                }
            },
            blackBox = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Colour4.Black,
                Alpha = 0
            }
        };

        maps.MapSetAdded += _ => playButton.Description = $"{maps.MapSets.Count} maps loaded";
    }

    private void exit()
    {
        cursorOverlay.ShowCursor = false;
        Conductor.FadeOut(2000);
        blackBox.FadeTo(1, 2000).OnComplete(_ => FluXisGame.ExitGame());
    }

    public override void OnEntering(ScreenTransitionEvent e)
    {
        this.FadeInFromZero(100);
        base.OnEntering(e);
    }

    public override void OnSuspending(ScreenTransitionEvent e)
    {
        this.FadeOut(100);
        content.ScaleTo(1.1f, 400, Easing.OutQuint);
        base.OnSuspending(e);
    }

    public override void OnResuming(ScreenTransitionEvent e)
    {
        this.FadeIn(100);
        content.ScaleTo(1f, 400, Easing.OutQuint);
        backgrounds.Zoom = 1;
        base.OnResuming(e);
    }
}
