using fluXis.Game.Graphics.UserInterface.Color;
using fluXis.Game.Skinning.Bases;
using fluXis.Game.Skinning.Json;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Game.Skinning.Default.Receptor;

public partial class DefaultReceptorUp : ColorableSkinDrawable
{
    protected Container Diamond { get; }

    public DefaultReceptorUp(SkinJson skinJson)
        : base(skinJson)
    {
        RelativeSizeAxes = Axes.X;
        Anchor = Anchor.BottomCentre;
        Origin = Anchor.BottomCentre;
        InternalChildren = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = FluXisColors.Background3
            },
            Diamond = new Container
            {
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Size = new Vector2(24, 24),
                Masking = true,
                CornerRadius = 5,
                BorderColour = FluXisColors.Background1,
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
