using System;
using fluXis.Game.Graphics;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Input;
using fluXis.Game.Online.Fluxel;
using fluXis.Game.Overlay.Chat;
using fluXis.Game.Overlay.Music;
using fluXis.Game.Overlay.Network;
using fluXis.Game.Overlay.Settings;
using fluXis.Game.Overlay.Toolbar.Buttons;
using fluXis.Game.Screens;
using fluXis.Game.Screens.Browse;
using fluXis.Game.Screens.Menu;
using fluXis.Game.Screens.Ranking;
using fluXis.Game.Screens.Select;
using fluXis.Game.Screens.Wiki;
using fluXis.Game.Utils;
using Humanizer;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
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
    private FluxelClient fluxel { get; set; }

    [Resolved]
    private FluXisGameBase game { get; set; }

    [Resolved]
    private FluXisScreenStack screens { get; set; }

    public BindableBool ShowToolbar { get; } = new();
    public BindableBool AllowOverlays { get; } = new();

    public override bool PropagateNonPositionalInputSubTree => AllowOverlays.Value;

    private string centerTextString;
    private FluXisSpriteText centerText;

    private double lastTime;

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
                        Name = "Screens (+Settings)",
                        Direction = FillDirection.Horizontal,
                        RelativeSizeAxes = Axes.Y,
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        AutoSizeAxes = Axes.X,
                        Children = new Drawable[]
                        {
                            new ToolbarOverlayButton
                            {
                                TooltipTitle = "Settings",
                                TooltipSub = "Change your settings.",
                                Icon = FontAwesome6.Solid.Gear,
                                Overlay = settings,
                                Keybind = FluXisGlobalKeybind.ToggleSettings
                            },
                            new ToolbarSeparator(),
                            new ToolbarScreenButton
                            {
                                TooltipTitle = "Home",
                                TooltipSub = "Return to the main menu.",
                                Icon = FontAwesome6.Solid.House,
                                Screen = typeof(MenuScreen),
                                Action = () => game.MenuScreen?.MakeCurrent(),
                                Keybind = FluXisGlobalKeybind.Home
                            },
                            new ToolbarScreenButton
                            {
                                TooltipTitle = "Maps",
                                TooltipSub = "Browse your maps.",
                                Icon = FontAwesome6.Solid.Map,
                                Screen = typeof(SelectScreen),
                                Action = () => goToScreen(new SelectScreen())
                            },
                            new ToolbarScreenButton
                            {
                                TooltipTitle = "Download Maps",
                                TooltipSub = "Download new maps.",
                                Icon = FontAwesome6.Solid.Download,
                                Screen = typeof(MapBrowser),
                                Action = () => goToScreen(new MapBrowser()),
                                RequireLogin = true
                            },
                            new ToolbarScreenButton
                            {
                                TooltipTitle = "Ranking",
                                TooltipSub = "See the top players.",
                                Icon = FontAwesome6.Solid.Trophy,
                                Screen = typeof(Rankings),
                                Action = () => goToScreen(new Rankings()),
                                RequireLogin = true
                            }
                        }
                    },
                    centerText = new FluXisSpriteText
                    {
                        Text = centerTextString,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Alpha = string.IsNullOrEmpty(centerTextString) ? 0 : 1
                    },
                    new FillFlowContainer
                    {
                        Name = "Overlays",
                        Direction = FillDirection.Horizontal,
                        RelativeSizeAxes = Axes.Y,
                        AutoSizeAxes = Axes.X,
                        Anchor = Anchor.CentreRight,
                        Origin = Anchor.CentreRight,
                        Children = new Drawable[]
                        {
                            new ToolbarOverlayButton
                            {
                                TooltipTitle = "Chat",
                                TooltipSub = "Talk to other players.",
                                Icon = FontAwesome6.Solid.Message,
                                Overlay = chat,
                                RequireLogin = true
                            },
                            new ToolbarOverlayButton
                            {
                                TooltipTitle = "Dashboard",
                                TooltipSub = "See news, updates, and more.",
                                Icon = FontAwesome6.Solid.ChartLine,
                                Overlay = dashboard,
                                Keybind = FluXisGlobalKeybind.ToggleDashboard,
                                RequireLogin = true
                            },
                            new ToolbarScreenButton
                            {
                                TooltipTitle = "Wiki",
                                TooltipSub = "Learn about the game.",
                                Icon = FontAwesome6.Solid.Book,
                                Screen = typeof(Wiki),
                                Action = () => goToScreen(new Wiki())
                            },
                            new ToolbarOverlayButton
                            {
                                TooltipTitle = "Music Player",
                                TooltipSub = "Listen to your music.",
                                Icon = FontAwesome6.Solid.Music,
                                Overlay = musicPlayer,
                                Keybind = FluXisGlobalKeybind.ToggleMusicPlayer,
                                Margin = new MarginPadding { Right = 10 }
                            },
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
        if (screens.CurrentScreen is not null && screens.CurrentScreen.GetType() == screen.GetType())
            return;

        if (screens.CurrentScreen is FluXisScreen { AllowExit: false })
            return;

        game.MenuScreen.MakeCurrent();
        screens.Push(screen);
    }

    private void OnShowToolbarChanged(ValueChangedEvent<bool> e)
    {
        if (e.OldValue == e.NewValue) return;

        this.MoveToY(e.NewValue ? 0 : -Height, 500, Easing.OutQuint);
    }

    public void SetCenterText(string text)
    {
        centerTextString = text;

        if (centerText == null)
            return;

        if (string.IsNullOrEmpty(text))
            centerText.FadeOut(400, Easing.OutQuint);
        else
        {
            centerText.Text = text;
            centerText.FadeIn(400, Easing.OutQuint);
        }
    }

    protected override void Update()
    {
        base.Update();

        if (Time.Current - lastTime < 1000)
            return;

        lastTime = Time.Current;

        if (fluxel.MaintenanceTime > 0)
        {
            var time = TimeUtils.GetFromSeconds(fluxel.MaintenanceTime);
            var timeLeft = time - DateTimeOffset.UtcNow;
            SetCenterText($"Server maintenance in {timeLeft.Humanize()}.");

            if (time <= DateTimeOffset.UtcNow)
            {
                fluxel.MaintenanceTime = 0;
                SetCenterText(string.Empty);
            }
        }
    }
}
