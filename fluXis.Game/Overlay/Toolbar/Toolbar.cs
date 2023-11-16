using fluXis.Game.Graphics;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Overlay.Chat;
using fluXis.Game.Overlay.Music;
using fluXis.Game.Overlay.Network;
using fluXis.Game.Overlay.Notifications;
using fluXis.Game.Overlay.Settings;
using fluXis.Game.Screens;
using fluXis.Game.Screens.Browse;
using fluXis.Game.Screens.Ranking;
using fluXis.Game.Screens.Select;
using fluXis.Game.Screens.Wiki;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Screens;

namespace fluXis.Game.Overlay.Toolbar;

public partial class Toolbar : Container
{
    [Resolved]
    private SettingsMenu settings { get; set; }

    [Resolved]
    private MusicPlayer musicPlayer { get; set; }

    [Resolved]
    private Dashboard dashboard { get; set; }

    [Resolved]
    private ChatOverlay chat { get; set; }

    [Resolved]
    private NotificationManager notifications { get; set; }

    [Resolved]
    private FluXisGameBase game { get; set; }

    public BindableBool ShowToolbar = new();

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Height = 60;
        Y = -60;
        Padding = new MarginPadding { Bottom = 10 };

        Children = new Drawable[]
        {
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                Masking = true,
                Child = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = FluXisColors.Background2
                },
                EdgeEffect = FluXisStyles.ShadowMediumNoOffset
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
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        AutoSizeAxes = Axes.X,
                        Children = new Drawable[]
                        {
                            new ToolbarButton
                            {
                                Title = "Home",
                                Description = "Return to the main menu.",
                                Icon = FontAwesome.Solid.Home,
                                Action = () => game.MenuScreen?.MakeCurrent()
                            },
                            new ToolbarSeperator(),
                            new ToolbarButton
                            {
                                Title = "Maps",
                                Description = "Browse your maps.",
                                Icon = FontAwesome.Solid.Map,
                                Action = () => goToScreen(new SelectScreen())
                            },
                            new ToolbarButton
                            {
                                Title = "Settings",
                                Description = "Change your settings.",
                                Icon = FontAwesome.Solid.Cog,
                                Action = () => settings.ToggleVisibility()
                            },
                            new ToolbarSeperator(),
                            new ToolbarButton
                            {
                                Title = "Chat",
                                Description = "Talk to other players.",
                                Icon = FontAwesome.Solid.Comment,
                                Action = chat.Show
                            },
                            new ToolbarButton
                            {
                                Title = "Ranking",
                                Description = "See the top players.",
                                Icon = FontAwesome.Solid.Trophy,
                                Action = () => goToScreen(new Rankings())
                            },
                            new ToolbarButton
                            {
                                Title = "Download Maps",
                                Description = "Download new maps.",
                                Icon = FontAwesome.Solid.Download,
                                Action = () => goToScreen(new MapBrowser())
                            },
                            new ToolbarButton
                            {
                                Title = "Dashboard",
                                Description = "See news, updates, and more.",
                                Icon = FontAwesome.Solid.ChartLine,
                                Action = dashboard.ToggleVisibility
                            },
                            new ToolbarButton
                            {
                                Title = "Wiki",
                                Description = "Learn about the game.",
                                Icon = FontAwesome.Solid.Book,
                                Action = () => goToScreen(new Wiki())
                            },
                            new ToolbarButton
                            {
                                Title = "Music Player",
                                Description = "Listen to your music.",
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
                        Anchor = Anchor.CentreRight,
                        Origin = Anchor.CentreRight,
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
        if (game.ScreenStack.CurrentScreen is not null && game.ScreenStack.CurrentScreen.GetType() == screen.GetType())
        {
            notifications.SendText("You are already on this screen.");
            return;
        }

        if (game.ScreenStack.CurrentScreen is FluXisScreen { AllowExit: false }) return;

        game.MenuScreen.MakeCurrent();
        game.ScreenStack.Push(screen);
    }

    private void OnShowToolbarChanged(ValueChangedEvent<bool> e)
    {
        if (e.OldValue == e.NewValue) return;

        this.MoveToY(e.NewValue ? 0 : -Height, 500, Easing.OutQuint);
    }
}
