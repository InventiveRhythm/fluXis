using System;
using System.Collections.Generic;
using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Buttons;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Graphics.UserInterface.Footer;
using fluXis.Game.UI;

namespace fluXis.Game.Screens.Course;

public partial class CourseScreenFooter : Footer
{
    public Action ExitAction { get; init; }
    public Action ContinueAction { get; init; }

    protected override CornerButton CreateLeftButton() => new()
    {
        ButtonText = "Exit",
        Icon = FontAwesome6.Solid.DoorOpen,
        Corner = Corner.BottomLeft,
        ButtonColor = FluXisColors.Background4,
        Action = ExitAction,
        PlayClickSound = false
    };

    protected override CornerButton CreateRightButton() => new()
    {
        ButtonText = "Continue",
        Icon = FontAwesome6.Solid.Play,
        Corner = Corner.BottomRight,
        ButtonColor = FluXisColors.Accent2,
        Action = ContinueAction,
        PlayClickSound = false
    };

    protected override IEnumerable<FooterButton> CreateButtons() => Array.Empty<FooterButton>();
}
