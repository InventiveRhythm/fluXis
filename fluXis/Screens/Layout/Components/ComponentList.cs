using System.Linq;
using fluXis.Graphics.Containers;
using fluXis.Screens.Gameplay.HUD;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osuTK;

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
            ScrollbarVisible = false,
            Child = flow = new FillFlowContainer
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Direction = FillDirection.Vertical,
                Spacing = new Vector2(16)
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
        // artificial delay because the health processor sometimes isn't set
        Scheduler.AddDelayed(() => flow.ChildrenEnumerable = manager.ComponentTypes.Select(pair => new LayoutListComponent(pair.Key, pair.Value)), 200);
    }
}
