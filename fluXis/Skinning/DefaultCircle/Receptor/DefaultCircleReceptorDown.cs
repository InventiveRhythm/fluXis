using fluXis.Graphics.UserInterface.Color;
using fluXis.Skinning.Json;
using osu.Framework.Graphics;

namespace fluXis.Skinning.DefaultCircle.Receptor;

public partial class DefaultCircleReceptorDown : DefaultCircleReceptorUp
{
    public DefaultCircleReceptorDown(SkinJson skinJson, MapColor index)
        : base(skinJson, index)
    {
        Alpha = 0;
        Circle.BorderThickness = 16;
    }

    public override void Show() => this.FadeInFromZero();
}
