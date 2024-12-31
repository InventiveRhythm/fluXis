using System;
using fluXis.Graphics.UserInterface.Color;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Graphics.UserInterface;

public partial class FlashLayer : Box
{
    public FlashLayer()
    {
        RelativeSizeAxes = Axes.Both;
        Colour = FluXisColors.Text;
        Alpha = 0;
    }

    public override void Show() => TransformableExtensions.FadeOutFromOne(this, 1000, Easing.OutQuint);

    [Obsolete]
    public void FadeOutFromOne(double duration, Easing ease) { }
}
