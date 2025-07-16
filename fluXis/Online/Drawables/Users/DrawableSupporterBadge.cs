using fluXis.Graphics.Sprites.Icons;
using fluXis.Graphics.UserInterface.Color;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Online.Drawables.Users;

public partial class DrawableSupporterBadge : CircularContainer
{
    [BackgroundDependencyLoader]
    private void load()
    {
        Size = new Vector2(32, 16);
        Masking = true;

        InternalChildren = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Colour4.FromHex("#FF99DC")
            },
            new FluXisSpriteIcon()
            {
                Size = new Vector2(10),
                Icon = FontAwesome6.Solid.Heart,
                Colour = FluXisColors.Background2,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre
            }
        };
    }
}
