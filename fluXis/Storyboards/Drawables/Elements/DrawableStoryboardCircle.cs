using System;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Storyboards.Drawables.Elements;

public partial class DrawableStoryboardCircle : DrawableStoryboardElement
{
    public DrawableStoryboardCircle(StoryboardElement element)
        : base(element)
    {
        if (element.Type != StoryboardElementType.Circle)
            throw new ArgumentException($"Element provided is not {nameof(StoryboardElementType.Circle)}", nameof(element));
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChild = new Circle
        {
            RelativeSizeAxes = Axes.Both
        };
    }
}
