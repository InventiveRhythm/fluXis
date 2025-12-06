using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Graphics.UserInterface;

public partial class VerticalSectionedGradient : CompositeDrawable
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
                Colour = ColourInfo.GradientVertical(Colour4.White.Opacity(EndAlpha), Colour4.White),
                Height = 1f - SplitPoint
            },
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                RelativePositionAxes = Axes.Y,
                Height = SplitPoint,
                Y = 1f - SplitPoint
            }
        };
    }
}
