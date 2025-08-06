using System;
using System.Linq;
using fluXis.Graphics.Containers;
using fluXis.Graphics.UserInterface.Color;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace fluXis.Overlay.Network.Sidebar;

public partial class DashboardSidebar : ExpandingContainer
{
    public const int SIZE_CLOSED = 64 + padding * 2;
    private const int padding = 5;
    private const int size_open = 200 + padding * 2;

    protected override double HoverDelay => 200;

    public Action<DashboardTab> SelectAction { get; init; }

    private FillFlowContainer<DashboardSidebarButton> content;

    [BackgroundDependencyLoader]
    private void load()
    {
        Width = 64;
        RelativeSizeAxes = Axes.Y;

        InternalChildren = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Theme.Background3
            },
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                Padding = new MarginPadding(padding),
                Child = content = new FillFlowContainer<DashboardSidebarButton>
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Anchor = Anchor.BottomLeft,
                    Origin = Anchor.BottomLeft,
                    Direction = FillDirection.Vertical,
                    Spacing = new Vector2(0, padding)
                }
            }
        };
    }

    public void AddTab(DashboardTab tab)
    {
        var button = new DashboardSidebarButton
        {
            Tab = tab,
            SelectAction = SelectAction
        };

        content.Add(button);
    }

    public DashboardSidebarButton GetButton(DashboardTab tab) => content.FirstOrDefault(b => b.Tab == tab);

    protected override void LoadComplete()
    {
        base.LoadComplete();

        Expanded.BindValueChanged(expanded =>
        {
            this.ResizeWidthTo(expanded.NewValue ? size_open : SIZE_CLOSED, 600, Easing.OutQuint);
        }, true);
    }
}
