using System;
using System.Collections.Generic;
using fluXis.Graphics.Sprites;
using fluXis.Graphics.UserInterface.Buttons;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Footer;
using fluXis.Localization;
using fluXis.UI;

namespace fluXis.Screens.Multiplayer.SubScreens.Open.List;

public partial class MultiLobbyListFooter : Footer
{
    public Action BackAction { get; init; }
    public Action RefreshAction { get; init; }
    public Action CreateAction { get; init; }

    protected override CornerButton CreateLeftButton() => new()
    {
        ButtonText = LocalizationStrings.General.Back,
        Icon = FontAwesome6.Solid.AngleLeft,
        Action = BackAction
    };

    protected override CornerButton CreateRightButton() => new()
    {
        ButtonText = "Refresh",
        Icon = FontAwesome6.Solid.ArrowsRotate,
        ButtonColor = FluXisColors.Primary,
        Corner = Corner.BottomRight,
        Action = RefreshAction
    };

    protected override IEnumerable<FooterButton> CreateButtons()
    {
        yield return new FooterButton
        {
            Text = "Create",
            Icon = FontAwesome6.Solid.Plus,
            AccentColor = FluXisColors.Footer1,
            Action = CreateAction
        };
    }
}
