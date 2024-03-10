using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Shared.Components.Groups;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Game.Online.Drawables;

public partial class DrawableGroupBadge : CircularContainer
{
    private IAPIGroup group { get; }

    public DrawableGroupBadge(IAPIGroup group)
    {
        this.group = group;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        Size = new Vector2(40, 16);
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
