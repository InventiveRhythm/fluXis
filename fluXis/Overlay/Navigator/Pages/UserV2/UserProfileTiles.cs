using System.Collections.Generic;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Overlay.Navigator.Pages.UserV2;

public partial class UserProfileTiles : GridContainer
{
    public UserProfileTiles(params UserProfileStatTile[] tiles)
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;

        var cols = new List<Dimension>();
        var content = new List<Drawable>();

        for (var i = 0; i < tiles.Length; i++)
        {
            if (i != 0) cols.Add(new Dimension(GridSizeMode.Absolute, 12));
            cols.Add(new Dimension());

            if (i != 0) content.Add(Empty());
            content.Add(tiles[i]);
        }

        ColumnDimensions = [.. cols];
        RowDimensions = [new Dimension(GridSizeMode.AutoSize)];
        Content = new[] { content.ToArray() };
    }
}
