using System;
using System.Collections.Generic;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.UserInterface.Buttons;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Footer;
using fluXis.Localization;
using fluXis.UI;

namespace fluXis.Screens.Result;

public partial class ResultsFooter : Footer
{
    public Action BackAction { get; init; }
    public Action ViewReplayAction { get; init; }
    public Action RestartAction { get; init; }

    protected override CornerButton CreateLeftButton() => new()
    {
        ButtonText = LocalizationStrings.General.Back,
        Icon = FontAwesome6.Solid.AngleLeft,
        Action = BackAction
    };

    protected override CornerButton CreateRightButton()
    {
        if (RestartAction is null)
            return null;

        return new CornerButton
        {
            ButtonText = "Retry",
            Corner = Corner.BottomRight,
            Icon = FontAwesome6.Solid.RotateRight,
            ButtonColor = FluXisColors.Primary,
            Action = RestartAction
        };
    }

    protected override IEnumerable<FooterButton> CreateButtons()
    {
        if (ViewReplayAction is not null)
        {
            yield return new FooterButton
            {
                Text = "View Replay",
                Icon = FontAwesome6.Solid.Film,
                AccentColor = FluXisColors.Footer1,
                Action = ViewReplayAction
            };
        }
    }
}
