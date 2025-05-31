using fluXis.Graphics.Sprites.Text;
using fluXis.Graphics.UserInterface.Color;
using fluXis.Online.API.Models.Groups;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Online.Drawables.Users;

public partial class DrawableGroupBadge : CircularContainer
{
    private APIGroup group { get; }

    public DrawableGroupBadge(APIGroup group)
    {
        this.group = group;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        Size = new Vector2(48, 16);
        Masking = true;

        InternalChildren = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = FluXisColors.Background2,
                Alpha = .8f
            },
            new FluXisSpriteText
            {
                Text = group.Tag,
                Colour = Colour4.FromHex(group.Color),
                WebFontSize = 10,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre
            }
        };
    }
}
