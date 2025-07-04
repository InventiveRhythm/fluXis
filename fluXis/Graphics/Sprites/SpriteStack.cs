using System.Linq;
using fluXis.Screens;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace fluXis.Graphics.Sprites;

public partial class SpriteStack<T> : CompositeDrawable
    where T : Drawable
{
    public T Current => InternalChildren.LastOrDefault() as T;

    public bool AutoFill { get; set; } = true;

    public SpriteStack()
    {
        RelativeSizeAxes = Axes.Both;
    }

    public void Add(T sprite, float duration = FluXisScreen.FADE_DURATION)
    {
        if (sprite is not IHasLoadedValue)
            Current?.Delay(duration).Expire();

        sprite.RelativeSizeAxes = Axes.Both;
        sprite.Anchor = sprite.Origin = Anchor.Centre;

        if (AutoFill)
            sprite.FillMode = FillMode.Fill;

        AddInternal(sprite);
    }

    protected override void Update()
    {
        base.Update();

        if (Current is not IHasLoadedValue)
            return;

        for (var i = 0; i < InternalChildren.Count; i++)
        {
            var child = InternalChildren[i];

            if (child == Current)
                continue;

            var next = InternalChildren[i + 1];
            bool invalid;

            if (next is IHasLoadedValue loaded)
                invalid = loaded.Loaded;
            else
                invalid = next.Alpha >= 1;

            if (invalid)
                child.Expire();
        }
    }
}
