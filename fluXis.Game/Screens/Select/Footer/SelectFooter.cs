using fluXis.Game.Input;
using fluXis.Game.Overlay.Notification;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osu.Framework.Screens;
using osuTK;

namespace fluXis.Game.Screens.Select.Footer;

public partial class SelectFooter : Container
{
    [Resolved]
    public NotificationOverlay Notifications { get; private set; }

    public SelectScreen Screen { get; init; }

    private Container keyboardContainer;
    private Container gamepadContainer;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Height = 50;
        Anchor = Origin = Anchor.BottomLeft;

        Children = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Colour4.Black,
                Alpha = .5f
            },
            keyboardContainer = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Padding = new MarginPadding { Horizontal = 10 },
                Alpha = GamepadHandler.GamepadConnected ? 0 : 1,
                Children = new Drawable[]
                {
                    new FillFlowContainer
                    {
                        Direction = FillDirection.Horizontal,
                        RelativeSizeAxes = Axes.Y,
                        AutoSizeAxes = Axes.X,
                        Anchor = Anchor.BottomLeft,
                        Origin = Anchor.BottomLeft,
                        Children = new Drawable[]
                        {
                            new SelectFooterButton
                            {
                                Text = "Back",
                                Action = Screen.Exit
                            },
                            new Container
                            {
                                RelativeSizeAxes = Axes.Y,
                                Width = 100,
                                Name = "Spacer"
                            },
                            new SelectFooterButton
                            {
                                Text = "Mods",
                                Action = openModSelector
                            },
                            new SelectFooterButton
                            {
                                Text = "Random",
                                Action = randomMap
                            },
                            new SelectFooterButton
                            {
                                Text = "Options",
                                Action = OpenSettings
                            }
                        }
                    },
                    new FillFlowContainer
                    {
                        Direction = FillDirection.Horizontal,
                        RelativeSizeAxes = Axes.Y,
                        AutoSizeAxes = Axes.X,
                        Anchor = Anchor.BottomRight,
                        Origin = Anchor.BottomRight,
                        Children = new Drawable[]
                        {
                            new SelectFooterButton
                            {
                                Text = "Play",
                                Action = Screen.Accept
                            }
                        }
                    }
                }
            },
            gamepadContainer = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Padding = new MarginPadding { Horizontal = 20 },
                Alpha = GamepadHandler.GamepadConnected ? 1 : 0,
                Children = new Drawable[]
                {
                    new FillFlowContainer
                    {
                        Direction = FillDirection.Horizontal,
                        RelativeSizeAxes = Axes.Y,
                        AutoSizeAxes = Axes.X,
                        Spacing = new Vector2(20),
                        Children = new Drawable[]
                        {
                            new SelectGamepadTooltip
                            {
                                Text = "Back",
                                Icon = "B"
                            },
                            new SelectGamepadTooltip
                            {
                                Text = "Mods",
                                Icon = "X"
                            },
                            new SelectGamepadTooltip
                            {
                                Text = "Random",
                                Icon = "Y"
                            },
                            new SelectGamepadTooltip
                            {
                                Text = "Options",
                                Icon = "Menu"
                            },
                        }
                    },
                    new FillFlowContainer
                    {
                        Direction = FillDirection.Horizontal,
                        RelativeSizeAxes = Axes.Y,
                        AutoSizeAxes = Axes.X,
                        Anchor = Anchor.BottomRight,
                        Origin = Anchor.BottomRight,
                        Spacing = new Vector2(20),
                        Children = new Drawable[]
                        {
                            new SelectGamepadTooltip
                            {
                                Text = "Change Map",
                                Icons = new[] { "DpadLeft", "DpadRight" }
                            },
                            new SelectGamepadTooltip
                            {
                                Text = "Change Difficulty",
                                Icons = new[] { "DpadUp", "DpadDown" }
                            },
                            new SelectGamepadTooltip
                            {
                                Text = "Select",
                                Icon = "A"
                            }
                        }
                    }
                }
            }
        };

        GamepadHandler.OnGamepadStatusChanged += updateGamepadStatus;
    }

    private void updateGamepadStatus(bool status)
    {
        keyboardContainer.FadeTo(status ? 0 : 1);
        gamepadContainer.FadeTo(status ? 1 : 0);
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);
        GamepadHandler.OnGamepadStatusChanged -= updateGamepadStatus;
    }

    private void openModSelector()
    {
        Screen.ModSelector.IsOpen.Value = true;
    }

    private void randomMap()
    {
        Screen.RandomMap();
    }

    public void OpenSettings()
    {
        Notifications.Post("This is still in development\nCome back later!");
    }

    protected override bool OnClick(ClickEvent e) => true; // Prevents the click from going through to the map list
}
