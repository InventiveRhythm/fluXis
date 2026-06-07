using fluXis.Online.API.Models.Maps.Modding;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Overlay.MapSet.UI.Modding;

public partial class ModdingActionComment : Container
{
    private readonly APIModdingAction action;

    public ModdingActionComment(APIModdingAction action)
    {
        this.action = action;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        AutoSizeAxes = Axes.Y;
        RelativeSizeAxes = Axes.X;

        InternalChild = new GridContainer
        {
            RelativeSizeAxes = Axes.X,
            AutoSizeAxes = Axes.Y,
            ColumnDimensions = new[]
            {
                new Dimension(GridSizeMode.AutoSize),
                new Dimension(GridSizeMode.Absolute, 8),
                new Dimension(GridSizeMode.Distributed),
            },
            RowDimensions = new[]
            {
                new Dimension(GridSizeMode.AutoSize),
            },
            Content = new[]
            {
                new Drawable[]
                {
                    new ModdingActionCommentLeftSide(action.Type),
                    null, //this represents the gap between the left side and the content box
                    new ModdingActionCommentContent(action)
                }
            }
        };
    }
}
