using System.Collections.Generic;
using System.Linq;
using fluXis.Graphics.Sprites.Text;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

namespace fluXis.Screens.Edit.Tabs.Setup;

public partial class SetupSection : FillFlowContainer
{
    public SetupSection(string title, Drawable[] entries)
    {
        RelativeSizeAxes = Axes.X;
        AutoSizeAxes = Axes.Y;
        Spacing = new Vector2(12);
        Direction = FillDirection.Vertical;

        InternalChildrenEnumerable = new FluXisSpriteText
        {
            Text = title,
            WebFontSize = 24,
            Margin = new MarginPadding { Horizontal = 12, Bottom = -8 }
        }.Yield().Concat(entries);
    }

    public partial class Row : GridContainer
    {
        public Row(Drawable[] items)
        {
            RelativeSizeAxes = Axes.X;
            AutoSizeAxes = Axes.Y;

            var dims = new List<Dimension>();
            var draws = new List<Drawable>();

            for (int i = 0; i < items.Length; i++)
            {
                if (i != 0)
                {
                    dims.Add(new Dimension(GridSizeMode.Absolute, 12));
                    draws.Add(Empty());
                }

                dims.Add(new Dimension());
                draws.Add(items[i]);
            }

            ColumnDimensions = dims.ToArray();
            RowDimensions = [new Dimension(GridSizeMode.AutoSize)];
            Content = new[] { draws.ToArray() };
        }
    }
}
