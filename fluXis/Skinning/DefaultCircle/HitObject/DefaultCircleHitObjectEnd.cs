using fluXis.Graphics.UserInterface.Color;
using fluXis.Skinning.Json;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;

namespace fluXis.Skinning.DefaultCircle.HitObject;

public partial class DefaultCircleHitObjectEnd : DefaultCircleHitObjectPiece
{
    public DefaultCircleHitObjectEnd(SkinJson skinJson, MapColor index)
        : base(skinJson, index)
    {
        Circle.BorderColour = ColourInfo.GradientVertical(Colour4.White.Darken(0.2f), Colour4.White);
        Circle.Child.Colour = Colour4.White.Darken(0.2f);
    }
}
