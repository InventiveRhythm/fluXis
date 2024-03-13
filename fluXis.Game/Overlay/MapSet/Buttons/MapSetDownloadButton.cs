using fluXis.Game.Graphics.UserInterface.Color;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osuTK;

namespace fluXis.Game.Overlay.MapSet.Buttons;

public partial class MapSetDownloadButton : CircularContainer
{
    private Box background;
    private SpriteIcon icon;

    [BackgroundDependencyLoader]
    private void load()
    {
        Size = new Vector2(48);
        Masking = true;

        Children = new Drawable[]
        {
            background = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = FluXisColors.ButtonGreen
            },
            icon = new SpriteIcon
            {
                Icon = FontAwesome.Solid.ArrowDown,
                Size = new Vector2(24),
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre
            }
        };
    }
}
