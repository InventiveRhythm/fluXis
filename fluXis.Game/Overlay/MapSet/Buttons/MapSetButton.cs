using System;
using fluXis.Game.Graphics.UserInterface.Color;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osuTK;

namespace fluXis.Game.Overlay.MapSet.Buttons;

public partial class MapSetButton : CircularContainer
{
    private IconUsage icon { get; }
    private Action action { get; }

    public MapSetButton(IconUsage icon, Action action)
    {
        this.icon = icon;
        this.action = action;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        Size = new Vector2(48);
        Masking = true;

        Children = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = FluXisColors.Background2
            },
            new SpriteIcon
            {
                Icon = icon,
                Size = new Vector2(24),
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre
            }
        };
    }
}
