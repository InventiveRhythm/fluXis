using System.Linq;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Game.Graphics.Sprites;

public partial class SpriteStack<T> : CompositeDrawable
    where T : Drawable
{
    public T Current => InternalChildren.LastOrDefault() as T;

    public bool AutoFill { get; set; } = true;

    public SpriteStack()
    {
        RelativeSizeAxes = Axes.Both;
    }

    public void Add(T sprite, float duration = 300)
    {
        Current?.Delay(duration).Expire();

        sprite.RelativeSizeAxes = Axes.Both;
        sprite.Anchor = sprite.Origin = Anchor.Centre;

        if (AutoFill)
            sprite.FillMode = FillMode.Fill;

        AddInternal(sprite);
    }
}
