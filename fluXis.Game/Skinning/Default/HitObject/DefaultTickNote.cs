using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Game.Skinning.Default.HitObject;

public partial class DefaultTickNote : CompositeDrawable
{
    public DefaultTickNote(bool small)
    {
        Width = small ? 0.8f : 1f;

        RelativeSizeAxes = Axes.X;
        Anchor = Anchor.BottomCentre;
        Origin = Anchor.BottomCentre;
        Colour = Colour4.FromHex("#F2C979");
        Height = 20;
        CornerRadius = 10;
        Masking = true;
        InternalChild = new Box
        {
            RelativeSizeAxes = Axes.Both
        };
    }
}
