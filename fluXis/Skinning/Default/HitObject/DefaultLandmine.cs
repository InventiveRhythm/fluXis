using fluXis.Graphics.Sprites.Outline;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Skinning.Default.HitObject;

public partial class DefaultLandmine : CompositeDrawable
{
    public DefaultLandmine()
    {
        Colour = Colour4.FromHex("#FF5252");
        Height = 42;
        InternalChild = new OutlinedSquare
        {
            RelativeSizeAxes = Axes.Both,
            BorderThickness = 10,
            BorderColour = Colour4.White,
            CornerRadius = 10
        };
    }

    protected override void Update()
    {
        var factor = DrawWidth / 114f;
        Height = 42f * factor;
    }
}
