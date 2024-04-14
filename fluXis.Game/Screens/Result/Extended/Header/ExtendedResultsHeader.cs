using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Game.Screens.Result.Extended.Header;

public partial class ExtendedResultsHeader : GridContainer
{
    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;
        Anchor = Origin = Anchor.Centre;

        ColumnDimensions = new Dimension[]
        {
            new(),
            new(GridSizeMode.Absolute, ExtendedResults.SPACING),
            new(GridSizeMode.AutoSize) // buttons
        };

        Content = new[]
        {
            new[]
            {
                new HeaderMap(),
                /*Empty(),
                new HeaderButtons()*/
            }
        };
    }
}
