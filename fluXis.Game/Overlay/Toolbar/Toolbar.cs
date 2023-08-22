using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Overlay.Chat;
using fluXis.Game.Overlay.Music;
using fluXis.Game.Overlay.Notification;
using fluXis.Game.Overlay.Settings;
using fluXis.Game.Screens;
using fluXis.Game.Screens.Browse;
using fluXis.Game.Screens.Dashboard;
using fluXis.Game.Screens.Menu;
using fluXis.Game.Screens.Ranking;
using fluXis.Game.Screens.Select;
using fluXis.Game.Screens.Wiki;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Screens;
using osuTK;

namespace fluXis.Game.Overlay.Toolbar;

public partial class Toolbar : Container
{
    [Resolved]
    private SettingsMenu settings { get; set; }

    [Resolved]
    private MusicPlayer musicPlayer { get; set; }

    [Resolved]
    private ChatOverlay chat { get; set; }

    [Resolved]
    private NotificationOverlay notifications { get; set; }

    public FluXisScreenStack ScreenStack { get; set; }
    public MenuScreen MenuScreen { get; set; }

    public BindableBool ShowToolbar = new();

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Height = 50;
        Y = -50;
        Padding = new MarginPadding { Vertical = 5, Horizontal = 10 };

        Children = new Drawable[]
        {
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                Masking = true,
                CornerRadius = 5,
                Shear = new Vector2(0.1f, 0),
                Child = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = FluXisColors.Background2
                },
                EdgeEffect = new EdgeEffectParameters
                {
                    Type = EdgeEffectType.Shadow,
                    Colour = Colour4.Black.Opacity(0.2f),
                    Radius = 5,
                    Offset = new Vector2(0, 1)
                }
            },
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                Padding = new MarginPadding { Horizontal = 10 },
                Children = new Drawable[]
                {
                    new FillFlowContainer
                    {
                        Direction = FillDirection.Horizontal,
                        RelativeSizeAxes = Axes.Y,
                        AutoSizeAxes = Axes.X,
                        Children = new Drawable[]
                        {
                            new ToolbarButton
                            {
                                Name = "Home",
                                Icon = FontAwesome.Solid.Home,
                                Action = () => MenuScreen?.MakeCurrent()
                            },
                            new ToolbarSeperator(),
                            new ToolbarButton
                            {
                                Name = "Maps",
                                Icon = FontAwesome.Solid.Map,
                                Action = () => goToScreen(new SelectScreen())
                            },
                            new ToolbarButton
                            {
                                Name = "Settings",
                                Icon = FontAwesome.Solid.Cog,
                                Action = () => settings.ToggleVisibility()
                            },
                            new ToolbarSeperator(),
                            new ToolbarButton
                            {
                                Name = "Chat",
                                Icon = FontAwesome.Solid.Comment,
                                Action = chat.Show
                            },
                            new ToolbarButton
                            {
                                Name = "Ranking",
                                Icon = FontAwesome.Solid.Trophy,
                                Action = () => goToScreen(new Rankings())
                            },
                            new ToolbarButton
                            {
                                Name = "Download Maps",
                                Icon = FontAwesome.Solid.Download,
                                Action = () => goToScreen(new MapBrowser())
                            },
                            new ToolbarButton
                            {
                                Name = "Dashboard",
                                Icon = FontAwesome.Solid.ChartLine,
                                Action = () => goToScreen(new Dashboard())
                            },
                            new ToolbarButton
                            {
                                Name = "Wiki",
                                Icon = FontAwesome.Solid.Book,
                                Action = () => goToScreen(new Wiki())
                            },
                            new ToolbarButton
                            {
                                Name = "Music Player",
                                Icon = FontAwesome.Solid.Music,
                                Action = musicPlayer.ToggleVisibility
                            }
                        }
                    },
                    new FillFlowContainer
                    {
                        Direction = FillDirection.Horizontal,
                        RelativeSizeAxes = Axes.Y,
                        AutoSizeAxes = Axes.X,
                        Anchor = Anchor.TopRight,
                        Origin = Anchor.TopRight,
                        Children = new Drawable[]
                        {
                            new ToolbarProfile(),
                            new ToolbarClock()
                        }
                    }
                }
            }
        };

        ShowToolbar.BindValueChanged(OnShowToolbarChanged, true);
    }

    private void goToScreen(IScreen screen)
    {
        if (ScreenStack.CurrentScreen is not null && ScreenStack.CurrentScreen.GetType() == screen.GetType())
        {
            notifications.Post("You are already on this screen.");
            return;
        }

        if (ScreenStack.CurrentScreen is FluXisScreen { AllowExit: false }) return;

        MenuScreen.MakeCurrent();
        ScreenStack.Push(screen);
    }

    private void OnShowToolbarChanged(ValueChangedEvent<bool> e)
    {
        if (e.OldValue == e.NewValue) return;

        this.MoveToY(e.NewValue ? 0 : -Height, 500, Easing.OutQuint);
    }
}
