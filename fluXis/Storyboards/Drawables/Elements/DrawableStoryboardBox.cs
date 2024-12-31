using System;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Storyboards.Drawables.Elements;

public partial class DrawableStoryboardBox : DrawableStoryboardElement
{
    public DrawableStoryboardBox(StoryboardElement element)
        : base(element)
    {
        if (element.Type != StoryboardElementType.Box)
            throw new ArgumentException("Element provided is not a box", nameof(element));
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChild = new Box
        {
            RelativeSizeAxes = Axes.Both
        };
    }
}
