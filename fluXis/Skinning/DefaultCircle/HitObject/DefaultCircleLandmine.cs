using fluXis.Graphics.Sprites.Outline;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Skinning.DefaultCircle.HitObject;

public partial class DefaultCircleLandmine : CompositeDrawable
{
    public DefaultCircleLandmine()
    {
        RelativeSizeAxes = Axes.X;
        Anchor = Anchor.BottomCentre;
        Origin = Anchor.BottomCentre;
        Colour = Colour4.FromHex("#FF5252");
        InternalChild = new OutlinedCircle
        {
            RelativeSizeAxes = Axes.Both,
            Size = new Vector2(DefaultCircleSkin.SCALE),
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            BorderColour = Colour4.White,
            BorderThickness = 17f
        };
    }

    protected override void Update()
    {
        base.Update();
        Height = DrawWidth;
    }
}
