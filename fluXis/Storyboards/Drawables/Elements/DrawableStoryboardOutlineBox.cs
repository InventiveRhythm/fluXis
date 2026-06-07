using System;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Storyboards.Drawables.Elements;

public partial class DrawableStoryboardOutlineBox : DrawableStoryboardElement
{
    protected override bool AllowBorder => true;

    public DrawableStoryboardOutlineBox(StoryboardElement element)
        : base(element)
    {
        if (element.Type != StoryboardElementType.OutlineBox)
            throw new ArgumentException($"Element provided is not a {nameof(StoryboardElementType.OutlineBox)}", nameof(element));

        BorderColour = Colour4.White;
        BorderThickness = (float)Element.GetParameter<double>("border", 4);
        Masking = true;

        InternalChild = new Box
        {
            RelativeSizeAxes = Axes.Both,
            Colour = Colour4.Transparent
        };
    }
}
