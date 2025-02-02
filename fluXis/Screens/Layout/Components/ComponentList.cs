using System.Linq;
using fluXis.Graphics.Containers;
using fluXis.Screens.Gameplay.HUD;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Screens.Layout.Components;

public partial class ComponentList : CompositeDrawable
{
    [Resolved]
    private LayoutEditor editor { get; set; }

    [Resolved]
    private LayoutManager manager { get; set; }

    private FillFlowContainer flow;

    [BackgroundDependencyLoader]
    private void load()
    {
        Padding = new MarginPadding(20);

        InternalChild = new FluXisScrollContainer
        {
            RelativeSizeAxes = Axes.Both,
            Child = flow = new FillFlowContainer
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Direction = FillDirection.Vertical
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        editor.RulesetLoaded += addComponents;

        if (editor.RulesetIsLoaded)
            addComponents();
    }

    private void addComponents()
    {
        flow.ChildrenEnumerable = manager.ComponentTypes.Select(pair => new LayoutListComponent(pair.Key, pair.Value));
    }
}
