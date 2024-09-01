using System;
using System.Collections.Generic;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Buttons;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Graphics.UserInterface.Footer;
using fluXis.Game.Localization;
using fluXis.Game.UI;

namespace fluXis.Game.Screens.Result;

public partial class ResultsFooter : Footer
{
    public Action BackAction { get; init; }
    public Action RestartAction { get; init; }

    protected override CornerButton CreateLeftButton() => new()
    {
        ButtonText = LocalizationStrings.General.Back,
        Icon = FontAwesome6.Solid.ChevronLeft,
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
            ButtonColor = FluXisColors.Accent2,
            Action = RestartAction
        };
    }

    protected override IEnumerable<FooterButton> CreateButtons() => ArraySegment<FooterButton>.Empty;
}
