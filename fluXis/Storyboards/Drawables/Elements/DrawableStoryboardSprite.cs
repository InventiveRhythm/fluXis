using System;
using fluXis.Storyboards.Storage;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Sprites;

namespace fluXis.Storyboards.Drawables.Elements;

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

        var file = Element.Parameters["file"].ToObject<string>();
        Name = file;

        InternalChild = new Sprite
        {
            Texture = storage.Textures.Get(file)
        };
    }
}
