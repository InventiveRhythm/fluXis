using System;
using fluXis.Graphics.Sprites.Text;
using osu.Framework.Allocation;
using osu.Framework.Graphics;

namespace fluXis.Storyboards.Drawables.Elements;

public partial class DrawableStoryboardText : DrawableStoryboardElement
{
    public DrawableStoryboardText(StoryboardElement element)
        : base(element)
    {
        if (element.Type != StoryboardElementType.Text)
            throw new ArgumentException("Element provided is not a text", nameof(element));
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        AutoSizeAxes = Axes.Both;

        var text = Element.Parameters["text"].ToObject<string>() ?? string.Empty;
        var size = Element.Parameters.ContainsKey("size") ? Element.Parameters["size"].ToObject<float>() : 20;

        InternalChild = new FluXisSpriteText
        {
            Text = text,
            FontSize = size
        };
    }
}
