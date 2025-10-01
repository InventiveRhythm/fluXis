using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Skinning.Default.HitObject;

public partial class DefaultLandmine : CompositeDrawable
{
    public DefaultLandmine(bool small)
    {
        RelativeSizeAxes = Axes.X;
        Anchor = Anchor.BottomCentre;
        Origin = Anchor.BottomCentre;
        Colour = Colour4.FromHex("#FF5252");
        Height = 42;
        CornerRadius = 10;
        Masking = true;
        InternalChildren = new Drawable[]
        {
            new Box()
            {
                RelativeSizeAxes = Axes.X,
                Width = 1,
                Height = 10,
                Anchor = Anchor.TopLeft,
                Origin = Anchor.TopLeft,
            },
            new Box()
            {
                RelativeSizeAxes = Axes.X,
                Width = 1,
                Height = 10,
                Anchor = Anchor.BottomLeft,
                Origin = Anchor.BottomLeft,
            },
            new Box()
            {
                RelativeSizeAxes = Axes.Y,
                Width = 10,
                Height = 1,
                Anchor = Anchor.TopLeft,
                Origin = Anchor.TopLeft,
            },
            new Box()
            {
                RelativeSizeAxes = Axes.Y,
                Width = 10,
                Height = 1,
                Anchor = Anchor.TopRight,
                Origin = Anchor.TopRight,
            },
        };
    }

    protected override void Update()
    {
        var factor = DrawWidth / 114f;
        Height = 42f * factor;
    }
}
