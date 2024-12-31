using fluXis.Graphics.UserInterface.Color;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Skinning.Default.Stage;

public partial class DefaultStageBackground : Box
{
    public DefaultStageBackground()
    {
        RelativeSizeAxes = Axes.Both;
        Colour = Colour4.Black;
        Alpha = 0.8f;
    }
}

public partial class DefaultStageBackgroundTop : Box
{
    public DefaultStageBackgroundTop()
    {
        RelativeSizeAxes = Axes.X;
        Colour = ColourInfo.GradientVertical(Colour4.Black.Opacity(0), Colour4.Black);
        Alpha = 0.8f;
    }
}

public partial class DefaultStageBackgroundBottom : Box
{
    public DefaultStageBackgroundBottom()
    {
        RelativeSizeAxes = Axes.X;
        Colour = ColourInfo.GradientVertical(FluXisColors.Background3, FluXisColors.Background3.Opacity(0));
    }
}
