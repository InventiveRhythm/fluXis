using System;
using System.Collections.Generic;
using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.UserInterface.Buttons;
using fluXis.Graphics.UserInterface.Footer;
using fluXis.UI;

namespace fluXis.Screens.Spectator;

public partial class SpectatorFooter : Footer
{
    private readonly Action exitAction;

    public SpectatorFooter(Action exitAction)
    {
        this.exitAction = exitAction;
    }

    protected override CornerButton CreateLeftButton() => new()
    {
        ButtonText = "Exit",
        Icon = Phosphor.Bold.DoorOpen,
        Corner = Corner.BottomLeft,
        Action = exitAction,
        PlayClickSound = false
    };

    protected override CornerButton CreateRightButton() => null;
    protected override IEnumerable<FooterButton> CreateButtons() => [];
}
