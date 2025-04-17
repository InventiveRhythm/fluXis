using fluXis.Skinning.Json;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;

namespace fluXis.Skinning.DefaultCircle.HitObject;

public partial class DefaultCircleHitObjectEnd : DefaultCircleHitObjectPiece
{
    public DefaultCircleHitObjectEnd(SkinJson skinJson)
        : base(skinJson)
    {
        Circle.BorderColour = ColourInfo.GradientVertical(Colour4.White.Darken(0.2f), Colour4.White);
        Circle.Child.Colour = Colour4.White.Darken(0.2f);
    }
}
