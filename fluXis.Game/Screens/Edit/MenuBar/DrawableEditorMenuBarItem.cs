using fluXis.Game.Graphics.Menu;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.UserInterface;

namespace fluXis.Game.Screens.Edit.MenuBar;

public partial class DrawableEditorMenuBarItem : DrawableFluXisMenuItem
{
    protected override float TextSize => 22;

    public DrawableEditorMenuBarItem(MenuItem item)
        : base(item)
    {
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        Foreground.Anchor = Anchor.Centre;
        Foreground.Origin = Anchor.Centre;
        Foreground.Margin = new MarginPadding { Horizontal = 10 };
    }
}
