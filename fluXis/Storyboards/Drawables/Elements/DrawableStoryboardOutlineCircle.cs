using System;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;

namespace fluXis.Storyboards.Drawables.Elements;

public partial class DrawableStoryboardOutlineCircle : DrawableStoryboardElement
{
    protected override bool AllowBorder => true;

    public DrawableStoryboardOutlineCircle(StoryboardElement element)
        : base(element)
    {
        if (element.Type != StoryboardElementType.OutlineCircle)
            throw new ArgumentException($"Element provided is not {nameof(StoryboardElementType.OutlineCircle)}", nameof(element));

        BorderColour = Colour4.White;
        BorderThickness = 4;
        Masking = true;

        InternalChild = new Box
        {
            RelativeSizeAxes = Axes.Both,
            Colour = Colour4.Transparent
        };
    }

    protected override void Update()
    {
        base.Update();

        var min = Math.Min(DrawWidth, DrawHeight);
        CornerRadius = min / 2f;
    }
}
