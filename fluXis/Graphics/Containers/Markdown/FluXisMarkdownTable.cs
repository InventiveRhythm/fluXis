using fluXis.Graphics.UserInterface.Color;
using Markdig.Extensions.Tables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers.Markdown;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Graphics.Containers.Markdown;

public partial class FluXisMarkdownTable : MarkdownTable
{
    public FluXisMarkdownTable(Table table)
        : base(table)
    {
        Margin = new MarginPadding { Bottom = 16 };
    }

    protected override MarkdownTableCell CreateTableCell(TableCell cell, TableColumnDefinition definition, bool isHeading)
        => new FluXisTableCell(cell, definition, isHeading);

    private partial class FluXisTableCell : MarkdownTableCell
    {
        private TableCell cell { get; }
        private bool header { get; }

        public FluXisTableCell(TableCell cell, TableColumnDefinition definition, bool header)
            : base(cell, definition)
        {
            this.cell = cell;
            this.header = header;
            BorderThickness = 0;
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            if (header)
            {
                AddInternal(new Box
                {
                    RelativeSizeAxes = Axes.X,
                    Height = 2,
                    Colour = FluXisColors.Background1,
                    Anchor = Anchor.BottomCentre,
                    Origin = Anchor.BottomCentre,
                });
            }

            var row = (cell.Parent as TableRow)!;
            var table = (row.Parent as Table)!;
            var idx = table.IndexOf(row);

            var even = idx % 2 == 0;
            var box = (Box)InternalChildren[0];
            box.Colour = !even || header ? FluXisColors.Background2 : FluXisColors.Background1;
            box.Alpha = 1;
        }
    }
}
