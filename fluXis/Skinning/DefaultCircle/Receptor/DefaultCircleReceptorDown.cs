using fluXis.Skinning.Json;
using osu.Framework.Graphics;

namespace fluXis.Skinning.DefaultCircle.Receptor;

public partial class DefaultCircleReceptorDown : DefaultCircleReceptorUp
{
    public DefaultCircleReceptorDown(SkinJson skinJson)
        : base(skinJson)
    {
        Alpha = 0;
        Circle.BorderThickness = 16;
    }

    protected override void SetColor(Colour4 color) => Colour = color;

    public override void Show() => this.FadeInFromZero();
}
