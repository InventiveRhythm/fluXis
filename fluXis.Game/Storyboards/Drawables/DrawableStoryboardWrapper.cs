using System.Collections.Generic;
using System.Linq;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Timing;

namespace fluXis.Game.Storyboards.Drawables;

public partial class DrawableStoryboardWrapper : DrawSizePreservingFillContainer
{
    private IFrameBasedClock clock { get; }
    private DrawableStoryboard storyboard { get; }
    private StoryboardLayer layer { get; }

    public DrawableStoryboardWrapper(IFrameBasedClock clock, DrawableStoryboard storyboard, StoryboardLayer layer)
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

        Child = new Container
        {
            Size = TargetDrawSize,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
            Masking = true,
            Clock = clock,
            ChildrenEnumerable = getLayers()
        };
    }

    private IEnumerable<DrawableStoryboardLayer> getLayers()
    {
        var groups = storyboard.Storyboard.Elements.Where(e => e.Layer == layer)
                               .GroupBy(e => e.ZIndex).OrderBy(g => g.Key);

        foreach (var group in groups)
            yield return storyboard.GetLayer(layer, group.Key);
    }
}
