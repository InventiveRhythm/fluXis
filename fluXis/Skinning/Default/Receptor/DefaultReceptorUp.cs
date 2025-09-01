using fluXis.Graphics.UserInterface.Color;
using fluXis.Skinning.Bases;
using fluXis.Skinning.Json;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Skinning.Default.Receptor;

public partial class DefaultReceptorUp : ColorableSkinDrawable
{
    protected Container Diamond { get; }

    public DefaultReceptorUp(SkinJson skinJson, int index)
        : base(skinJson, index)
    {
        RelativeSizeAxes = Axes.X;
        Anchor = Anchor.BottomCentre;
        Origin = Anchor.BottomCentre;
        InternalChildren = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Theme.Background3
            },
            Diamond = new Container
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Size = new Vector2(24, 24),
                Masking = true,
                CornerRadius = 5,
                BorderColour = Theme.Background1,
                BorderThickness = 5,
                Rotation = 45,
                Child = new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    AlwaysPresent = true,
                    Alpha = 0
                }
            }
        };
    }
}
