using fluXis.Game.Graphics;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Game.Skinning.Default.Stage;

public partial class DefaultStageBorderRight : Container
{
    [BackgroundDependencyLoader]
    private void load()
    {
        AutoSizeAxes = Axes.X;
        RelativeSizeAxes = Axes.Y;
        Anchor = Anchor.TopRight;
        Origin = Anchor.TopLeft;

        Children = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Y,
                Width = 5,
                Colour = FluXisColors.Background3
            },
            new Box
            {
                RelativeSizeAxes = Axes.Y,
                Width = 2,
                Margin = new MarginPadding { Left = 5 },
                Alpha = .5f,
                Colour = FluXisColors.Accent
            }
        };
    }
}
