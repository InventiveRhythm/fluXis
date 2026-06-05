using System;
using System.Collections.Generic;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.UserInterface.Buttons;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Graphics.UserInterface.Footer;
using fluXis.UI;
using osu.Framework.Bindables;

namespace fluXis.Screens.Multiplayer.SubScreens.Open.Lobby;

public partial class MultiLobbyFooter : Footer
{
    public Action LeaveAction { get; init; }
    public Action RightButtonAction { get; init; }
    public Action ChangeMapAction { get; init; }
    public Action ViewMapAction { get; init; }

    public BindableBool CanChangeMap { get; } = new(true);

    protected override CornerButton CreateLeftButton() => new()
    {
        ButtonText = "Leave",
        Icon = Phosphor.Bold.DoorOpen,
        Action = LeaveAction
    };

    protected override CornerButton CreateRightButton() => new()
    {
        ButtonText = "Ready",
        Icon = Phosphor.Bold.CheckSquare,
        ButtonColor = Theme.Primary,
        Corner = Corner.BottomRight,
        Action = RightButtonAction
    };

    protected override IEnumerable<FooterButton> CreateButtons()
    {
        yield return new FooterButton
        {
            Text = "Change Map",
            Icon = Phosphor.Bold.ArrowsLeftRight,
            AccentColor = Theme.Footer1,
            Enabled = CanChangeMap,
            Action = ChangeMapAction
        };

        yield return new FooterButton
        {
            Text = "View Map",
            Icon = Phosphor.Bold.Eye,
            AccentColor = Theme.Footer2,
            Action = ViewMapAction
        };
    }
}
