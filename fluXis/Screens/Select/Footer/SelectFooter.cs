using System;
using System.Collections.Generic;
using fluXis.Database.Maps;
using fluXis.Graphics.Gamepad;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.UserInterface.Buttons;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Footer;
using fluXis.Input;
using fluXis.Localization;
using fluXis.Screens.Select.Footer.Options;
using fluXis.Screens.Select.Footer.Practice;
using fluXis.UI;
using osu.Framework.Graphics;
using osu.Framework.Input;
using osu.Framework.Input.Events;
using osuTK.Input;

namespace fluXis.Screens.Select.Footer;

#nullable enable

public partial class SelectFooter : Graphics.UserInterface.Footer.Footer
{
    public Action? BackAction { get; init; }
    public Action? ModsAction { get; init; }
    public Action? RandomAction { get; init; }
    public Action? RewindAction { get; init; }
    public Action<int, int>? PracticeAction { get; set; }
    public Action? PlayAction { get; init; }

    public Action<RealmMapSet>? ExportAction { get; init; }
    public Action<RealmMap>? EditAction { get; init; }
    public Action<RealmMapSet>? DeleteAction { get; init; }
    public Action? ScoresWiped { get; init; }

    private FooterButton randomButton = null!;
    private FooterOptions options = null!;
    private FooterPractice practice = null!;

    private InputManager inputManager = null!;

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
        ButtonColor = Theme.Primary,
        Corner = Corner.BottomRight,
        Action = PlayAction,
        PlayClickSound = false
    };

    protected override Drawable[] CreateBackgroundContent() => new Drawable[]
    {
        options = new FooterOptions
        {
            DeleteAction = DeleteAction,
            EditAction = EditAction,
            ExportAction = ExportAction,
            ScoresWiped = ScoresWiped,
        },
        practice = new FooterPractice
        {
            PracticeAction = PracticeAction
        }
    };

    protected override IEnumerable<FooterButton> CreateButtons()
    {
        yield return new SelectModsButton(ModsAction);

        yield return randomButton = new FooterButton
        {
            Text = LocalizationStrings.SongSelect.FooterRandom,
            Icon = FontAwesome6.Solid.Shuffle,
            AccentColor = Theme.Footer2,
            Action = () =>
            {
                if (inputManager.CurrentState.Keyboard.ShiftPressed)
                    RewindAction?.Invoke();
                else
                    RandomAction?.Invoke();
            },
            Index = 1
        };

        yield return options.Button = new FooterButton
        {
            Text = LocalizationStrings.SongSelect.FooterOptions,
            Icon = FontAwesome6.Solid.Gear,
            AccentColor = Theme.Footer3,
            Action = OpenSettings,
            Index = 2
        };

        if (PracticeAction is not null)
        {
            yield return practice.Button = new FooterButton
            {
                Text = LocalizationStrings.SongSelect.FooterPractice,
                Icon = FontAwesome6.Solid.BullseyeArrow,
                AccentColor = Theme.Footer4,
                Action = practice.ToggleVisibility,
                Index = 3
            };
        }
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

    protected override bool OnKeyDown(KeyDownEvent e)
    {
        if (e.Repeat)
            return false;

        switch (e.Key)
        {
            case >= Key.F1 and <= Key.F12:
            {
                var index = (int)e.Key - (int)Key.F1;

                if (index < Buttons.Count)
                    Buttons[index].TriggerClick();

                return true;
            }
        }

        return false;
    }

    public void OpenSettings() => options.ToggleVisibility();
}
