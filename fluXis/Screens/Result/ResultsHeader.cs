using fluXis.Screens.Result.Header;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Screens.Result;

public partial class ResultsHeader : CompositeDrawable
{
    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
        Padding = new MarginPadding(16);

        InternalChildren = new Drawable[]
        {
            new ResultsMap(),
            new ResultsPlayer
            {
                Anchor = Anchor.TopRight,
                Origin = Anchor.TopRight
            }
        };
    }
}
