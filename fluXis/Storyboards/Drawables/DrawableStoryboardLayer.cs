using System.Linq;
using fluXis.Storyboards.Drawables.Elements;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Timing;

namespace fluXis.Storyboards.Drawables;

public partial class DrawableStoryboardLayer : DrawSizePreservingFillContainer
{
    private IFrameBasedClock clock { get; }
    private DrawableStoryboard storyboard { get; }
    private StoryboardLayer layer { get; }
    private DependencyContainer dependencies;

    public DrawableStoryboardLayer(IFrameBasedClock clock, DrawableStoryboard storyboard, StoryboardLayer layer)
    {
        this.clock = clock;
        this.storyboard = storyboard;
        this.layer = layer;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        RelativeSizeAxes = Axes.Both;
        TargetDrawSize = storyboard.Storyboard.Resolution;

        dependencies.CacheAs(clock);
        dependencies.CacheAs(storyboard);
        dependencies.CacheAs(storyboard.Storage);

        Child = new DrawableStoryboardCompound(new StoryboardElement
        {
            Type = StoryboardElementType.Compound,
            Width = TargetDrawSize.X,
            Height = TargetDrawSize.Y,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre
        }, storyboard.Storyboard.Elements.Where(x => x.Layer == layer).ToList());
    }

    protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
        => dependencies = new DependencyContainer(base.CreateChildDependencies(parent));
}
