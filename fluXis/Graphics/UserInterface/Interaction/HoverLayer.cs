using System;
using fluXis.Graphics.UserInterface.Color;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Graphics.UserInterface.Interaction;

public partial class HoverLayer : Box
{
    public HoverLayer()
    {
        RelativeSizeAxes = Axes.Both;
        Colour = Theme.Text;
        Alpha = 0;
    }

    public override void Show() => TransformableExtensions.FadeTo(this, .2f, 50);
    public override void Hide() => TransformableExtensions.FadeOut(this, 200);

    [Obsolete]
    public void FadeTo(float newAlpha, double duration = 0, Easing ease = Easing.None) { }

    [Obsolete]
    public void FadeOut(double duration = 0, Easing ease = Easing.None) { }
}
