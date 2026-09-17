using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Graphics.UserInterface.Text;

public partial class TextArea
{
    protected partial class Line : FillFlowContainer
    {
        public int StartPosition { get; set; }
        public bool HasNewLine { get; set; }

        public Line()
        {
            AutoSizeAxes = Axes.X;
            Direction = FillDirection.Horizontal;
        }
    }
}
