using fluXis.Skinning.Default.HitObject;
using fluXis.Skinning.Json;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;

namespace fluXis.Skinning.DefaultCircle.HitObject;

public partial class DefaultCircleHitObjectBody : DefaultHitObjectBody
{
    public DefaultCircleHitObjectBody(SkinJson skinJson)
        : base(skinJson)
    {
        Width = DefaultCircleSkin.SCALE - 0.01f;
        Masking = true;
        BorderThickness = 8;
        BorderColour = ColourInfo.GradientVertical(Colour4.White.Darken(0.1f), Colour4.White);
    }
}
