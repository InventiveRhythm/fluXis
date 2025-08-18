using System.Collections.Generic;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Overlay.Settings.UI.Visual;

public partial class SettingsVisualRow : GridContainer
{
    public SettingsVisualRow(Drawable[] draws)
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
        RowDimensions = new Dimension[] { new(GridSizeMode.AutoSize) };

        var dims = new List<Dimension>();
        var content = new List<Drawable>();

        foreach (var draw in draws)
        {
            content.Add(draw);
            dims.Add(new Dimension());

            // content.Add(Empty());
            // dims.Add(new Dimension(GridSizeMode.Absolute, 16));
        }

        // dims.Remove(dims.Last());
        // content.Remove(content.Last());

        ColumnDimensions = dims.ToArray();
        Content = new[] { content.ToArray() };
    }
}
