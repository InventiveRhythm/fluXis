using fluXis.Skinning.Json;
using osu.Framework.Graphics;

namespace fluXis.Skinning.DefaultCircle.Receptor;

public partial class DefaultCircleReceptorDown : DefaultCircleReceptorUp
{
    public DefaultCircleReceptorDown(SkinJson skinJson, int index)
        : base(skinJson, index)
    {
        Alpha = 0;
        Circle.BorderThickness = 16;
    }

    public override void SetColor(Colour4 color) => Colour = color;

    public override void FadeColor(Colour4 color, double duration = 0, Easing easing = Easing.None)
        => this.FadeColour(color, duration, easing);

    public override void Show() => this.FadeInFromZero();
}
