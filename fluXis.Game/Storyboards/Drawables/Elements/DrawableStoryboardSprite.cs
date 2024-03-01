using System;
using fluXis.Game.Storyboards.Storage;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Game.Storyboards.Drawables.Elements;

public partial class DrawableStoryboardSprite : DrawableStoryboardElement
{
    public DrawableStoryboardSprite(StoryboardElement element)
        : base(element)
    {
        if (element.Type != StoryboardElementType.Sprite)
            throw new ArgumentException("Element provided is not a sprite", nameof(element));
    }

    [BackgroundDependencyLoader]
    private void load(StoryboardStorage storage)
    {
        AutoSizeAxes = Axes.Both;
        Name = Element.Parameters["file"];

        InternalChild = new Sprite
        {
            Texture = storage.Textures.Get(Element.Parameters["file"])
        };
    }
}
