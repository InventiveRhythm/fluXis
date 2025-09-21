using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Skinning.Default.HitObject;

public partial class DefaultLandmine : CompositeDrawable
{
    public DefaultLandmine(bool small)
    {
        Width = small ? 0.8f : 1f;

        RelativeSizeAxes = Axes.X;
        Anchor = Anchor.BottomCentre;
        Origin = Anchor.BottomCentre;
        Colour = Colour4.FromHex("#FF5252");
        Height = 20;
        CornerRadius = 10;
        Masking = true;
        InternalChild = new Box
        {
            RelativeSizeAxes = Axes.Both
        };
    }
}
