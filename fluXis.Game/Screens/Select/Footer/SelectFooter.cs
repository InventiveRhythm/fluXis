using System;
using fluXis.Game.Graphics;
using fluXis.Game.Graphics.Gamepad;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Buttons;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Input;
using fluXis.Game.Screens.Select.Footer.Options;
using fluXis.Game.UI;
using osu.Framework.Allocation;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input;
using osu.Framework.Input.Events;
using osu.Framework.Screens;

namespace fluXis.Game.Screens.Select.Footer;

public partial class SelectFooter : Container
{
    public SelectScreen Screen { get; init; }
    public Container<SelectFooterButton> ButtonContainer { get; private set; }

    public Action ScoresWiped { get; init; }

    private Container backgroundContainer;
    private Container keyboardContainer;
    private GamepadTooltipBar gamepadContainer;
    private CornerButton backButton;
    private CornerButton playButton;
    private SelectFooterButton randomButton;
    private FooterOptions options;

    private InputManager inputManager;

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
                EdgeEffect = FluXisStyles.ShadowMediumNoOffset,
                Child = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = FluXisColors.Background2
                }
            },
            options = new FooterOptions
            {
                Footer = this,
                ScoresWiped = ScoresWiped
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
                        Icon = FontAwesome6.Solid.ChevronLeft,
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
                                Icon = FontAwesome6.Solid.LayerGroup,
                                AccentColor = Colour4.FromHex("#edbb98"),
                                Action = openModSelector
                            },
                            randomButton = new SelectFooterButton
                            {
                                Text = "Random",
                                Icon = FontAwesome6.Solid.Shuffle,
                                AccentColor = Colour4.FromHex("#ed98a7"),
                                Action = randomMap,
                                Index = 1,
                                Margin = new MarginPadding { Left = 160 }
                            },
                            options.Button = new SelectFooterButton
                            {
                                Text = "Options",
                                Icon = FontAwesome6.Solid.Gear,
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
                        Icon = FontAwesome6.Solid.Play,
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

    protected override void LoadComplete()
    {
        base.LoadComplete();
        inputManager = GetContainingInputManager();
    }

    protected override void Update()
    {
        base.Update();

        randomButton.Text = inputManager.CurrentState.Keyboard.ShiftPressed ? "Rewind" : "Random";
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

    protected override bool OnHover(HoverEvent e)
    {
        return true;
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

    private void randomMap()
    {
        if (inputManager.CurrentState.Keyboard.ShiftPressed)
            Screen.RewindRandom();
        else
            Screen.RandomMap();
    }

    public void OpenSettings()
    {
        options.ToggleVisibility();
    }

    protected override bool OnClick(ClickEvent e) => true; // Prevents the click from going through to the map list
}
