using System;
using System.Collections.Generic;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Buttons;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Graphics.UserInterface.Footer;
using fluXis.Game.Localization;
using fluXis.Game.UI;
using osu.Framework.Graphics;

namespace fluXis.Game.Screens.Multiplayer.SubScreens.Open.List;

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
        ButtonColor = FluXisColors.Accent2,
        Corner = Corner.BottomRight,
        Action = RefreshAction
    };

    protected override IEnumerable<FooterButton> CreateButtons()
    {
        yield return new FooterButton
        {
            Text = "Create",
            Icon = FontAwesome6.Solid.Plus,
            AccentColor = Colour4.FromHex("#edbb98"),
            Action = CreateAction
        };
    }
}
