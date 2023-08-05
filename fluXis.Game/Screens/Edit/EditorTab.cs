using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Game.Screens.Edit;

public partial class EditorTab : Container
{
    public Editor Screen { get; set; }

    public EditorTab(Editor screen)
    {
        Screen = screen;
        Alpha = 0;
        RelativeSizeAxes = Axes.Both;
        RelativePositionAxes = Axes.Both;
    }
}
