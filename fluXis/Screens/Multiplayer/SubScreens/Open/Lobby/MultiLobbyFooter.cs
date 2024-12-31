using System;
using System.Collections.Generic;
using fluXis.Graphics.Sprites;
using fluXis.Graphics.UserInterface.Buttons;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Footer;
using fluXis.UI;
using osu.Framework.Bindables;
using osu.Framework.Graphics;

namespace fluXis.Screens.Multiplayer.SubScreens.Open.Lobby;

public partial class MultiLobbyFooter : Footer
{
    public Action LeaveAction { get; init; }
    public Action RightButtonAction { get; init; }
    public Action ChangeMapAction { get; init; }

    public BindableBool CanChangeMap { get; } = new(true);

    protected override CornerButton CreateLeftButton() => new()
    {
        ButtonText = "Leave",
        Icon = FontAwesome6.Solid.DoorOpen,
        Action = LeaveAction
    };

    protected override CornerButton CreateRightButton() => new()
    {
        ButtonText = "Ready",
        Icon = FontAwesome6.Solid.SquareCheck,
        ButtonColor = FluXisColors.Primary,
        Corner = Corner.BottomRight,
        Action = RightButtonAction
    };

    protected override IEnumerable<FooterButton> CreateButtons()
    {
        yield return new FooterButton
        {
            Text = "Change Map",
            Icon = FontAwesome6.Solid.ArrowRightArrowLeft,
            AccentColor = Colour4.FromHex("#edbb98"),
            Enabled = CanChangeMap,
            Action = ChangeMapAction
        };
    }
}
