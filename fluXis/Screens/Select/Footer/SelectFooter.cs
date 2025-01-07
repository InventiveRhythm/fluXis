using System;
using System.Collections.Generic;
using fluXis.Database.Maps;
using fluXis.Graphics.Gamepad;
using fluXis.Graphics.Sprites;
using fluXis.Graphics.UserInterface.Buttons;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Footer;
using fluXis.Input;
using fluXis.Localization;
using fluXis.Screens.Select.Footer.Options;
using fluXis.UI;
using osu.Framework.Graphics;
using osu.Framework.Input;

namespace fluXis.Screens.Select.Footer;

public partial class SelectFooter : Graphics.UserInterface.Footer.Footer
{
    public Action BackAction { get; init; }
    public Action ModsAction { get; init; }
    public Action RandomAction { get; init; }
    public Action RewindAction { get; init; }
    public Action PlayAction { get; init; }

    public Action<RealmMapSet> DeleteAction { get; init; }
    public Action<RealmMap> EditAction { get; init; }
    public Action ScoresWiped { get; init; }

    private FooterButton randomButton;
    private FooterOptions options;

    private InputManager inputManager;

    protected override void LoadComplete()
    {
        base.LoadComplete();
        inputManager = GetContainingInputManager();
    }

    protected override void Update()
    {
        base.Update();

        randomButton.Text = inputManager.CurrentState.Keyboard.ShiftPressed
            ? LocalizationStrings.SongSelect.FooterRewind
            : LocalizationStrings.SongSelect.FooterRandom;
    }

    #region Overrides

    protected override CornerButton CreateLeftButton() => new()
    {
        ButtonText = LocalizationStrings.General.Back,
        Icon = FontAwesome6.Solid.AngleLeft,
        Action = BackAction,
        PlayClickSound = false
    };

    protected override CornerButton CreateRightButton() => new()
    {
        ButtonText = LocalizationStrings.General.Play,
        Icon = FontAwesome6.Solid.Play,
        ButtonColor = FluXisColors.Primary,
        Corner = Corner.BottomRight,
        Action = PlayAction,
        PlayClickSound = false
    };

    protected override Drawable CreateBackgroundContent() => options = new FooterOptions
    {
        DeleteAction = DeleteAction,
        EditAction = EditAction,
        ScoresWiped = ScoresWiped
    };

    protected override IEnumerable<FooterButton> CreateButtons()
    {
        return new[]
        {
            new SelectModsButton(ModsAction),
            randomButton = new FooterButton
            {
                Text = LocalizationStrings.SongSelect.FooterRandom,
                Icon = FontAwesome6.Solid.Shuffle,
                AccentColor = Colour4.FromHex("#ed98a7"),
                Action = randomMap,
                Index = 1
            },
            options.Button = new FooterButton
            {
                Text = LocalizationStrings.SongSelect.FooterOptions,
                Icon = FontAwesome6.Solid.Gear,
                AccentColor = Colour4.FromHex("#98cbed"),
                Action = OpenSettings,
                Index = 2
            }
        };
    }

    protected override GamepadTooltipBar CreateGamepadTooltips() => new()
    {
        Alpha = GamepadHandler.GamepadConnected ? 1 : 0,
        ShowBackground = false,
        TooltipsLeft = new GamepadTooltip[]
        {
            new()
            {
                Text = LocalizationStrings.General.Back,
                Icon = "B"
            },
            new()
            {
                Text = LocalizationStrings.SongSelect.FooterMods,
                Icon = "X"
            },
            new()
            {
                Text = LocalizationStrings.SongSelect.FooterRandom,
                Icon = "Y"
            },
            new()
            {
                Text = LocalizationStrings.SongSelect.FooterOptions,
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
                Text = LocalizationStrings.General.Play,
                Icon = "A"
            }
        }
    };

    #endregion

    public void OpenSettings()
    {
        options.ToggleVisibility();
    }

    private void randomMap()
    {
        if (inputManager.CurrentState.Keyboard.ShiftPressed)
            RewindAction?.Invoke();
        else
            RandomAction?.Invoke();
    }
}
