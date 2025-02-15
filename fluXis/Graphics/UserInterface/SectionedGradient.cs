using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Graphics.UserInterface;

public partial class SectionedGradient : CompositeDrawable
{
    public float SplitPoint { get; init; } = .2f;
    public float EndAlpha { get; init; }

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChildren = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Width = SplitPoint
            },
            new Box
            {
                Colour = ColourInfo.GradientHorizontal(Colour4.White, Colour4.White.Opacity(EndAlpha)),
                RelativeSizeAxes = Axes.Both,
                RelativePositionAxes = Axes.X,
                Width = 1 - SplitPoint,
                X = SplitPoint
            }
        };
    }
}
