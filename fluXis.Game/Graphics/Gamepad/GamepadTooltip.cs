using System;
using System.Linq;
using fluXis.Game.Graphics.Sprites;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Localisation;
using osuTK;

namespace fluXis.Game.Graphics.Gamepad;

public partial class GamepadTooltip : FillFlowContainer
{
    public LocalisableString Text { get; init; } = string.Empty;
    public string[] Icons { get; init; } = Array.Empty<string>();
    public string Icon { init => Icons = new[] { value }; }

    [BackgroundDependencyLoader]
    private void load()
    {
        AutoSizeAxes = Axes.Both;
        Direction = FillDirection.Horizontal;
        Spacing = new Vector2(4);
        Anchor = Origin = Anchor.CentreLeft;

        InternalChildren = Icons.Select<string, Drawable>(i => new GamepadIcon
        {
            ButtonName = i,
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

