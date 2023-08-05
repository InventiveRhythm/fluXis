using fluXis.Game.Graphics;
using fluXis.Game.Graphics.Containers;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Game.Screens.Edit.Tabs.Charting;

public partial class Toolbox : ExpandingContainer
{
    private const int padding = 5;
    public const int SIZE_CLOSED = 48 + padding * 2;
    private const int size_open = 200 + padding * 2;

    protected override double HoverDelay => 500;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Y;

        Child = new Box
        {
            RelativeSizeAxes = Axes.Both,
            Colour = FluXisColors.Background2
        };
    }

    protected override void LoadComplete()
    {
        Expanded.BindValueChanged(v =>
        {
            this.ResizeWidthTo(v.NewValue ? size_open : SIZE_CLOSED, 500, Easing.OutQuint);
        }, true);
    }
}
