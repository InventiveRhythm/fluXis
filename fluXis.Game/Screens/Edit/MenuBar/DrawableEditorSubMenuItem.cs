using fluXis.Game.Graphics.Menu;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.UserInterface;

namespace fluXis.Game.Screens.Edit.MenuBar;

public partial class DrawableEditorSubMenuItem : DrawableFluXisMenuItem
{
    protected override float TextSize => 22;

    public DrawableEditorSubMenuItem(MenuItem item)
        : base(item)
    {
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        CornerRadius = 0;
        Foreground.Margin = new MarginPadding { Horizontal = 10 };
    }
}
