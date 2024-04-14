using fluXis.Game.Graphics.Sprites;
using fluXis.Game.Screens.Result.Extended.Header;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Game.Screens.Result.Extended;

public partial class ExtendedResults : Container
{
    public const float SPACING = 10;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;
        Anchor = Origin = Anchor.Centre;

        InternalChildren = new Drawable[]
        {
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                Padding = new MarginPadding(50) { Bottom = 100 },
                Child = new GridContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    RowDimensions = new Dimension[]
                    {
                        new(GridSizeMode.Absolute, 120),
                        new(GridSizeMode.Absolute, SPACING),
                        new()
                    },
                    Content = new[]
                    {
                        new Drawable[]
                        {
                            new ExtendedResultsHeader()
                        },
                        new[] { Empty() },
                        new Drawable[]
                        {
                            new GridContainer
                            {
                                RelativeSizeAxes = Axes.Both,
                                ColumnDimensions = new Dimension[]
                                {
                                    new(GridSizeMode.Absolute, 500),
                                    new(GridSizeMode.Absolute, SPACING),
                                    new()
                                },
                                Content = new[]
                                {
                                    new[]
                                    {
                                        new ExtendedResultsInfo(),
                                        Empty(),
                                        new Container
                                        {
                                            RelativeSizeAxes = Axes.Both,
                                            Child = new FluXisSpriteText
                                            {
                                                Text = "Graphs here soon...",
                                                Anchor = Anchor.Centre,
                                                Origin = Anchor.Centre
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            },
            new FluXisSpriteText
            {
                Text = "This is still not finished and will show more info soon\u2122.",
                Anchor = Anchor.TopCentre,
                Origin = Anchor.TopCentre,
                WebFontSize = 16,
                Y = 10
            }
        };
    }
}
