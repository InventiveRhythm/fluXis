using System;
using System.Collections.Generic;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Timing;

namespace fluXis.Game.Storyboards.Drawables;

public partial class DrawableStoryboardWrapper : DrawSizePreservingFillContainer
{
    private IFrameBasedClock clock { get; }
    private DrawableStoryboard storyboard { get; }
    private StoryboardLayerGroup layer { get; }

    public DrawableStoryboardWrapper(IFrameBasedClock clock, DrawableStoryboard storyboard, StoryboardLayerGroup layer)
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
        switch (layer)
        {
            case StoryboardLayerGroup.Background:
                yield return storyboard.GetLayer(StoryboardLayer.B4);
                yield return storyboard.GetLayer(StoryboardLayer.B3);
                yield return storyboard.GetLayer(StoryboardLayer.B2);
                yield return storyboard.GetLayer(StoryboardLayer.B1);

                break;

            case StoryboardLayerGroup.Foreground:
                yield return storyboard.GetLayer(StoryboardLayer.F1);
                yield return storyboard.GetLayer(StoryboardLayer.F2);

                break;

            case StoryboardLayerGroup.Overlay:
                yield return storyboard.GetLayer(StoryboardLayer.O1);
                yield return storyboard.GetLayer(StoryboardLayer.O2);

                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public enum StoryboardLayerGroup
    {
        Background,
        Foreground,
        Overlay
    }
}
