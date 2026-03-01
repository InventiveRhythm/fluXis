using fluXis.Graphics.UserInterface.Color;
using fluXis.Online.API.Models.Maps.Modding;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Overlay.MapSet.UI.Modding;

public partial class ModdingActionCommentLeftSide : GridContainer
{
    private readonly APIModdingActionType type;

    public ModdingActionCommentLeftSide(APIModdingActionType type)
    {
        this.type = type;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        AutoSizeAxes = Axes.X;
        RelativeSizeAxes = Axes.Y;

        ColumnDimensions = new[]
        {
            new Dimension(GridSizeMode.AutoSize),
        };

        RowDimensions = new[]
        {
            new Dimension(GridSizeMode.AutoSize),
            new Dimension(GridSizeMode.Absolute, 8),
            new Dimension(GridSizeMode.Distributed)
        };

        Content = new[]
        {
            new Drawable[] { new ModdingActionTypeCircle(type), },
            new Drawable[] { null }, // this represents the gap between the circle and the line
            new Drawable[]
            {
                new Container
                {
                    Anchor = Anchor.TopCentre,
                    Origin = Anchor.TopCentre,
                    Width = 4,
                    RelativeSizeAxes = Axes.Y,
                    CornerRadius = 2,
                    Masking = true,
                    Child = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = Theme.Background3
                    }
                }
            }
        };
    }
}
