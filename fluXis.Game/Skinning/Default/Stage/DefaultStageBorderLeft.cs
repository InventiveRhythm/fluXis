using fluXis.Game.Graphics;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Game.Skinning.Default.Stage;

public partial class DefaultStageBorderLeft : Container
{
    [BackgroundDependencyLoader]
    private void load()
    {
        AutoSizeAxes = Axes.X;
        RelativeSizeAxes = Axes.Y;

        Children = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Y,
                Width = 5,
                Margin = new MarginPadding { Left = 2 },
                Colour = FluXisColors.Surface
            },
            new Box
            {
                RelativeSizeAxes = Axes.Y,
                Width = 2,
                Alpha = .5f,
                Colour = FluXisColors.Accent3
            }
        };
    }
}
