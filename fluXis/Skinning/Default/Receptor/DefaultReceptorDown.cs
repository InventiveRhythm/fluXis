using fluXis.Skinning.Json;
using osu.Framework.Graphics;

namespace fluXis.Skinning.Default.Receptor;

public partial class DefaultReceptorDown : DefaultReceptorUp
{
    public DefaultReceptorDown(SkinJson skinJson)
        : base(skinJson)
    {
    }

    public override void SetColor(Colour4 color) => Diamond.BorderColour = color;

    public override void FadeColor(Colour4 color, double duration = 0, Easing easing = Easing.None)
        => Diamond.TransformTo(nameof(Diamond.BorderColour), color, duration, easing);
}
