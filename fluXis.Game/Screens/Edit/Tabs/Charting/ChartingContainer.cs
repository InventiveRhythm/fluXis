using System.Collections.Generic;
using fluXis.Game.Screens.Edit.Tabs.Charting.Blueprints;
using fluXis.Game.Screens.Edit.Tabs.Charting.Playfield;
using fluXis.Game.Screens.Edit.Tabs.Charting.Tools;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Game.Screens.Edit.Tabs.Charting;

public partial class ChartingContainer : Container
{
    public IReadOnlyList<ChartingTool> Tools { get; } = new ChartingTool[]
    {
        new SelectTool()
    };

    [Resolved]
    private EditorClock clock { get; set; }

    private DependencyContainer dependencies;

    public EditorPlayfield Playfield { get; private set; }
    public IEnumerable<EditorHitObject> HitObjects => Playfield.HitObjectContainer.HitObjects;

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;
        InternalChildren = new Drawable[]
        {
            new Container
            {
                Name = "Playfield",
                RelativeSizeAxes = Axes.Both,
                Children = new Drawable[]
                {
                    Playfield = new EditorPlayfield { Clock = clock },
                    new BlueprintContainer { ChartingContainer = this }
                }
            },
            new Toolbox()
        };

        dependencies.Cache(Playfield);
    }

    protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
    {
        return dependencies = new DependencyContainer(base.CreateChildDependencies(parent));
    }
}
