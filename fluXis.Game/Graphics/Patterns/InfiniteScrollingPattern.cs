using System;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osuTK;

namespace fluXis.Game.Graphics.Patterns;

public partial class InfiniteScrollingPattern : Container
{
    private Texture texture;

    public Texture Texture
    {
        get => texture;
        set
        {
            texture = value;

            // the initial pattern genreation hasnt been done yet, so we can skip this
            if (!hasLoaded) return;

            foreach (var drawable in Children)
                RemoveInternal(drawable, false);

            generatePattern();
        }
    }

    public Vector2 Speed { get; set; } = Vector2.One;
    public Vector2 TextureScale { get; set; } = Vector2.One;
    public Vector2 TextureSpacing { get; set; } = Vector2.Zero;
    public Vector2 Extent { get; set; } = Vector2.One;

    private bool hasLoaded;
    private int xCount, yCount;

    private void generatePattern()
    {
        if (texture == null)
            return;

        var spriteSize = texture.Size * TextureScale;
        var size = RelativeSizeAxes == Axes.Both ? DrawSize : new Vector2(DrawWidth, DrawHeight);

        xCount = (int)Math.Ceiling(size.X / (spriteSize.X + TextureSpacing.X)) + (int)Extent.X;
        yCount = (int)Math.Ceiling(size.Y / (spriteSize.Y + TextureSpacing.Y)) + (int)Extent.Y;

        for (int x = 0; x < xCount; x++)
        {
            for (int y = 0; y < yCount; y++)
            {
                var sprite = new Sprite
                {
                    Texture = texture,
                    Anchor = Anchor.TopLeft,
                    Origin = Anchor.TopLeft,
                    Position = new Vector2(x * spriteSize.X + x * TextureSpacing.X, y * spriteSize.Y + y * TextureSpacing.Y),
                    Size = spriteSize
                };

                Add(sprite);
            }
        }
    }

    protected override void LoadComplete()
    {
        generatePattern();
        hasLoaded = true;
        base.LoadComplete();
    }

    protected override void Update()
    {
        foreach (var child in Children)
        {
            child.Position += Speed * ((float)Clock.ElapsedFrameTime / 1000f);

            switch (Speed.X)
            {
                case > 0 when child.X > DrawWidth:
                    child.X -= child.Size.X * xCount;
                    break;

                case < 0 when child.X < -DrawWidth:
                    child.X += child.Size.X * xCount;
                    break;
            }

            switch (Speed.Y)
            {
                case > 0 when child.Y > DrawHeight:
                    child.Y -= child.Size.Y * yCount;
                    break;

                case < 0 when child.Y < -DrawHeight:
                    child.Y += child.Size.Y * yCount;
                    break;
            }
        }

        base.Update();
    }

    public void SpeedTo(Vector2 newSpeed, double duration = 0, Easing easing = Easing.None)
    {
        this.TransformTo(nameof(Speed), newSpeed, duration, easing);
    }
}
