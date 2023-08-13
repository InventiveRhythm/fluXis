using fluXis.Game.Graphics;
using fluXis.Game.Graphics.Gamepad;
using fluXis.Game.Graphics.UserInterface.Buttons;
using fluXis.Game.Input;
using fluXis.Game.Overlay.Notification;
using fluXis.Game.UI;
using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Input.Events;
using osu.Framework.Screens;
using osuTK.Graphics;

namespace fluXis.Game.Screens.Select.Footer;

public partial class SelectFooter : Container
{
    [Resolved]
    public NotificationOverlay Notifications { get; private set; }

    public SelectScreen Screen { get; init; }
    public Container<SelectFooterButton> ButtonContainer { get; private set; }

    private Container backgroundContainer;
    private Container keyboardContainer;
    private GamepadTooltipBar gamepadContainer;
    private CornerButton backButton;
    private CornerButton playButton;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        Height = 60;
        Anchor = Origin = Anchor.BottomLeft;

        Children = new Drawable[]
        {
            backgroundContainer = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Masking = true,
                Y = 80,
                EdgeEffect = new EdgeEffectParameters
                {
                    Type = EdgeEffectType.Shadow,
                    Colour = Color4.Black.Opacity(.25f),
                    Radius = 10
                },
                Child = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = FluXisColors.Background2
                }
            },
            keyboardContainer = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Padding = new MarginPadding { Horizontal = 10 },
                Alpha = GamepadHandler.GamepadConnected ? 0 : 1,
                Children = new Drawable[]
                {
                    backButton = new CornerButton
                    {
                        ButtonText = "Back",
                        Icon = FontAwesome.Solid.ChevronLeft,
                        Action = Screen.Exit
                    },
                    ButtonContainer = new Container<SelectFooterButton>
                    {
                        RelativeSizeAxes = Axes.Y,
                        AutoSizeAxes = Axes.X,
                        Y = 100,
                        Anchor = Anchor.BottomLeft,
                        Origin = Anchor.BottomLeft,
                        Padding = new MarginPadding { Left = 300 },
                        Children = new[]
                        {
                            new SelectFooterButton
                            {
                                Text = "Mods",
                                Icon = FontAwesome.Solid.LayerGroup,
                                AccentColor = Colour4.FromHex("#edbb98"),
                                Action = openModSelector
                            },
                            new SelectFooterButton
                            {
                                Text = "Random",
                                Icon = FontAwesome.Solid.Random,
                                AccentColor = Colour4.FromHex("#ed98a7"),
                                Action = randomMap,
                                Index = 1,
                                Margin = new MarginPadding { Left = 160 }
                            },
                            new SelectFooterButton
                            {
                                Text = "Options",
                                Icon = FontAwesome.Solid.Cog,
                                AccentColor = Colour4.FromHex("#98cbed"),
                                Action = OpenSettings,
                                Index = 2,
                                Margin = new MarginPadding { Left = 320 }
                            }
                        }
                    },
                    playButton = new CornerButton
                    {
                        ButtonText = "Play!",
                        Icon = FontAwesome.Solid.Play,
                        ButtonColor = FluXisColors.Accent2,
                        Corner = Corner.BottomRight,
                        Action = Screen.Accept
                    }
                }
            },
            gamepadContainer = new GamepadTooltipBar
            {
                Alpha = GamepadHandler.GamepadConnected ? 1 : 0,
                ShowBackground = false,
                TooltipsLeft = new GamepadTooltip[]
                {
                    new()
                    {
                        Text = "Back",
                        Icon = "B"
                    },
                    new()
                    {
                        Text = "Mods",
                        Icon = "X"
                    },
                    new()
                    {
                        Text = "Random",
                        Icon = "Y"
                    },
                    new()
                    {
                        Text = "Options",
                        Icon = "Menu"
                    }
                },
                TooltipsRight = new GamepadTooltip[]
                {
                    new()
                    {
                        Text = "Change Map",
                        Icons = new[] { "DpadLeft", "DpadRight" }
                    },
                    new()
                    {
                        Text = "Change Difficulty",
                        Icons = new[] { "DpadUp", "DpadDown" }
                    },
                    new()
                    {
                        Text = "Play",
                        Icon = "A"
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

    public override void Show()
    {
        backButton.Show();
        playButton.Show();
        backgroundContainer.MoveToY(0, 500, Easing.OutQuint);
        ButtonContainer.MoveToY(0);
        ButtonContainer.ForEach(b => b.Show());
    }

    public override void Hide()
    {
        backButton.Hide();
        playButton.Hide();
        backgroundContainer.MoveToY(80, 500, Easing.OutQuint);
        ButtonContainer.MoveToY(100, 500, Easing.OutQuint);
    }

    private void openModSelector() => Screen.ModSelector.IsOpen.Toggle();
    private void randomMap() => Screen.RandomMap();
    public void OpenSettings() => Notifications.Post("This is still in development\nCome back later!");

    protected override bool OnClick(ClickEvent e) => true; // Prevents the click from going through to the map list
}
