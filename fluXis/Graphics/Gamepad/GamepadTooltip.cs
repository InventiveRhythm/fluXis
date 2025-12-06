using System.ComponentModel;
using System.Linq;
using fluXis.Graphics.Sprites.Text;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Localisation;
using osuTK;

namespace fluXis.Graphics.Gamepad;

public partial class GamepadTooltip : FillFlowContainer
{
    public LocalisableString Text { get; }
    public ButtonGlyph[] Icons { get; }

    public GamepadTooltip(LocalisableString text, params ButtonGlyph[] icons)
    {
        Text = text;
        Icons = icons;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        AutoSizeAxes = Axes.Both;
        Direction = FillDirection.Horizontal;
        Spacing = new Vector2(4);
        Anchor = Origin = Anchor.CentreLeft;

        InternalChildren = Icons.Select<ButtonGlyph, Drawable>(i => new GamepadIcon(i)
        {
            Size = new Vector2(30)
        }).Append(new FluXisSpriteText
        {
            Text = Text,
            FontSize = 24,
            Anchor = Anchor.CentreLeft,
            Origin = Anchor.CentreLeft
        }).ToArray();
    }
}

public enum ButtonGlyph
{
    None = -1,

    A,
    B,
    X,
    Y,

    [Description("Dpad")]
    Pad,

    [Description("DpadDown")]
    PadDown,

    [Description("DpadUp")]
    PadUp,

    [Description("DpadLeft")]
    PadLeft,

    [Description("DpadRight")]
    PadRight,

    [Description("LB")]
    LeftBump,

    [Description("RB")]
    RightBump,

    [Description("LT")]
    LeftTrigger,

    [Description("RT")]
    RightTrigger,

    LeftStick,
    RightStick,
    LeftStickClick,
    RightStickClick,

    Menu,
    View,
    Share
}
