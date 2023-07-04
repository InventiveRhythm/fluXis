using System;
using fluXis.Game.Graphics;
using fluXis.Game.Graphics.Gamepad;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Game.Screens.Select.Footer;

public partial class SelectGamepadTooltip : FillFlowContainer
{
    public string Text { get; init; } = string.Empty;
    public string Icon { get; init; } = string.Empty;
    public string[] Icons { get; init; } = Array.Empty<string>();

    [BackgroundDependencyLoader]
    private void load()
    {
        AutoSizeAxes = Axes.Both;
        Direction = FillDirection.Horizontal;
        Spacing = new Vector2(4);
        Anchor = Origin = Anchor.CentreLeft;

        if (Icons.Length > 0)
        {
            foreach (var icon in Icons)
            {
                Add(new GamepadIcon
                {
                    ButtonName = icon,
                    Size = new Vector2(30)
                });
            }
        }
        else
        {
            Add(new GamepadIcon
            {
                ButtonName = Icon,
                Size = new Vector2(30)
            });
        }

        Add(new FluXisSpriteText
        {
            Text = Text,
            FontSize = 24,
            Anchor = Anchor.CentreLeft,
            Origin = Anchor.CentreLeft
        });
    }
}
