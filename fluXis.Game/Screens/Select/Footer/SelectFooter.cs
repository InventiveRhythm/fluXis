using System;
using System.Collections.Generic;
using fluXis.Game.Database.Maps;
using fluXis.Game.Graphics.Gamepad;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Buttons;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Graphics.UserInterface.Footer;
using fluXis.Game.Input;
using fluXis.Game.Localization;
using fluXis.Game.Screens.Select.Footer.Options;
using fluXis.Game.UI;
using osu.Framework.Graphics;
using osu.Framework.Input;

namespace fluXis.Game.Screens.Select.Footer;

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
        Icon = FontAwesome6.Solid.ChevronLeft,
        Action = BackAction,
        PlayClickSound = false
    };

    protected override CornerButton CreateRightButton() => new()
    {
        ButtonText = LocalizationStrings.General.Play,
        Icon = FontAwesome6.Solid.Play,
        ButtonColor = FluXisColors.Accent2,
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
            new()
            {
                Text = LocalizationStrings.SongSelect.FooterMods,
                Icon = FontAwesome6.Solid.LayerGroup,
                AccentColor = Colour4.FromHex("#edbb98"),
                Action = ModsAction,
            },
            randomButton = new FooterButton
            {
                Text = LocalizationStrings.SongSelect.FooterRandom,
                Icon = FontAwesome6.Solid.Shuffle,
                AccentColor = Colour4.FromHex("#ed98a7"),
                Action = randomMap,
                Index = 1,
                Margin = new MarginPadding { Left = 160 }
            },
            options.Button = new FooterButton
            {
                Text = LocalizationStrings.SongSelect.FooterOptions,
                Icon = FontAwesome6.Solid.Gear,
                AccentColor = Colour4.FromHex("#98cbed"),
                Action = OpenSettings,
                Index = 2,
                Margin = new MarginPadding { Left = 320 }
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
